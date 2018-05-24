using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Engine.Attribute;
using Engine.Geobase.Dirrect;
using Engine.Geobase.Marshal.DataDescription;
using Engine.Helpers;

namespace Engine.Geobase.Marshal
{
    public class GeobaseEngineMarshal: IGeobaseEngine
    {
        public readonly byte[] Bytes;

        private GeobaseDataMarshal _data;
        private int[] _cityIndexPrepared;

        private int Records => _data.Header.Records;

        public int HeaderLengh { get; }
        public int RangesLengh { get; }
        public int LocationLengh { get; }
        public int CityIndexLengh { get; }

        public GeobaseEngineMarshal(string fileName)
        {
            var stopwatchAll = Stopwatch.StartNew();

            HeaderLengh = Extentions.GetBytesLength(typeof(GeobaseHeaderMarshal));
            RangesLengh = Extentions.GetBytesLength(typeof(GeobaseIpRangeMarshal));
            LocationLengh = Extentions.GetBytesLength(typeof(GeobaseLocationMarshal));
            CityIndexLengh = Extentions.GetBytesLength(typeof(GeobaseCityIndexMarshal));

            Bytes = File.ReadAllBytes(fileName);

            //_data = ByteHelper.BytesToStruct<GeobaseDataMarshal>(Bytes); // old approach (slow, but simple)

            #region parrallel loading
            _data = new GeobaseDataMarshal();

            _data.Header = ByteHelper.BytesToStruct<GeobaseHeaderMarshal>(ByteHelper.GetBytes(Bytes, 0, HeaderLengh));

            var locationPartsCount = 50;
            var locationParts = new ConcurrentBag<GeobaseLocationsPart>();
            var locationTasks = new Task[locationPartsCount + 2];

            if (Records % locationPartsCount != 0)
            {
                throw new Exception("Wrong locationPartsCount");
            }
            var locationPartSize = Records / locationPartsCount;
            var locationPartSizeBytes = LocationLengh * locationPartSize;
            for (var i = 0; i < locationPartsCount; i++)
            {
                var i1 = i;
                locationTasks[i] =
                    Task.Factory.StartNew(
                        () =>
                        {
                            locationParts.Add(new GeobaseLocationsPart
                            {
                                Locations = ByteHelper.BytesToStruct<GeobaseLocationsMarshal>(
                                        ByteHelper.GetBytes(Bytes
                                                            , (int)_data.Header.OffsetLocations + locationPartSizeBytes * i1
                                                            , locationPartSizeBytes)).Locations
                                    ,
                                PartNumber = i1
                            });
                        })
                ;
            }

            locationTasks[locationPartsCount] = Task.Factory.StartNew(
                () => { _data.IpRanges = ByteHelper.BytesToStruct<GeobaseIpRangesMarshal>(ByteHelper.GetBytes(Bytes, (int)_data.Header.OffsetRanges, Records * RangesLengh)).IpRanges; });

            locationTasks[locationPartsCount + 1] = Task.Factory.StartNew(
                () => { _data.CityIndexes = ByteHelper.BytesToStruct<GeobaseCityIndexesMarshal>(ByteHelper.GetBytes(Bytes, (int)_data.Header.OffsetCities, Records * CityIndexLengh)).CityIndexes; });

            Task.WaitAll(locationTasks);

            _data.Locations = new GeobaseLocationMarshal[Records];
            var j = 0;
            foreach (var locationPart in locationParts.OrderBy(x => x.PartNumber))
            {
                Array.Copy(locationPart.Locations, 0, _data.Locations, j * locationPartSize, locationPartSize);
                j++;
            }

            #endregion

            _cityIndexPrepared = new int[Records];
            for (int i = 0; i < Records; i++)
            {
                _cityIndexPrepared[i] = (int)(_data.CityIndexes[i].LocationOffset / LocationLengh);
            }
            stopwatchAll.Stop();
            Debug.WriteLine("GeobaseEngineMarshal, DB loading time: " + stopwatchAll.Elapsed.TotalMilliseconds + " ms");
        }

        private GeobaseLocationView GetLocation(int index)
        {
            return TransformationHelper.GetLocationView(_data.Locations[index]);
        }

        public GeobaseLocationView FindLocationByIp(string ipString)
        {
            var stopwatch = Stopwatch.StartNew();

            var ipUint = IpHelper.IpStringToUint(ipString);
            int locactionIndex = -1;

            // binary search
            int min = 0;
            int max = _data.IpRanges.Length - 1;
            while (min <= max)
            {  
                int mid = (min + max) / 2;

                var ipFrom = _data.IpRanges[mid].IpFrom;
                var ipTo = _data.IpRanges[mid].IpTo;

                if (ipFrom <= ipUint && ipTo >= ipUint)
                {  
                    locactionIndex = (int)_data.IpRanges[mid].LocationIndex;
                    break;
                }
                else if (ipUint < ipFrom)
                {  
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            if (locactionIndex < 0)
            {
                Debug.WriteLine("GeobaseEngineMarshal, FindLocationByIp, ip = " + ipString + ", time: " + stopwatch.Elapsed.TotalMilliseconds + " ms. Result: NOT FOUND");
                return null;
            }

            Debug.WriteLine("GeobaseEngineMarshal, FindLocationByIp, ip = " + ipString + ", time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");

            return GetLocation(locactionIndex);
        }

        public IEnumerable<GeobaseLocationView> FindLocationsByCity(string city)
        {
            var stopwatch = Stopwatch.StartNew();

            // binary search
            int min = 0;
            int max = _data.CityIndexes.Length - 1;
            bool found = false;
            while (min <= max)
            {
                int mid = (min + max) / 2;

                var locationIndex = _cityIndexPrepared[mid];
                var locationCity = _data.Locations[locationIndex].City;

                var comparisonResult = string.CompareOrdinal(city, locationCity);

                if (comparisonResult == 0)
                {
                    yield return GetLocation(locationIndex);

                    var previousCityIndex = mid - 1;
                    if (previousCityIndex >= 0)
                    {
                        locationIndex = _cityIndexPrepared[previousCityIndex];
                        var previousCity = _data.Locations[locationIndex].City;

                        while (previousCityIndex >= 0 && previousCity == city)
                        {
                            if (previousCity == city)
                            {
                                yield return GetLocation(locationIndex);
                            }

                            previousCityIndex--;
                            if (previousCityIndex < 0)
                            {
                                break;
                            }
                            locationIndex = _cityIndexPrepared[previousCityIndex];
                            previousCity = _data.Locations[locationIndex].City;
                        }
                    }


                    var nextCityIndex = mid + 1;
                    if (nextCityIndex < _data.CityIndexes.Length)
                    {
                        locationIndex = _cityIndexPrepared[nextCityIndex];
                        var nextCity = _data.Locations[locationIndex].City;

                        while (nextCityIndex >= 0 && nextCity == city)
                        {
                            if (nextCity == city)
                            {
                                yield return GetLocation(locationIndex);
                            }

                            nextCityIndex++;
                            if (nextCityIndex >= _data.CityIndexes.Length)
                            {
                                break;
                            }
                            locationIndex = _cityIndexPrepared[nextCityIndex];
                            nextCity = _data.Locations[locationIndex].City;
                        }
                    }

                    found = true;
                    break;
                }
                else if (comparisonResult < 0)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            if (!found)
            {
                Debug.WriteLine("GeobaseEngineMarshal, FindLocationsByCity, city = " + city + " time: " + stopwatch.Elapsed.TotalMilliseconds + " ms. Result: NOT FOUND");
            }

            Debug.WriteLine("GeobaseEngineMarshal, FindLocationsByCity, city = " + city + " time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
        }
    }
}