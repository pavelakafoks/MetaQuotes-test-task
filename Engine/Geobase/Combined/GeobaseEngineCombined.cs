using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Engine.Attribute;
using Engine.Geobase.Combined.DataDescription;
using Engine.Geobase.Dirrect;
using Engine.Geobase.Dirrect.DataDescription;
using Engine.Geobase.Marshal.DataDescription;
using Engine.Helpers;

namespace Engine.Geobase.Combined
{
    public class GeobaseEngineCombined: IGeobaseEngine
    {
        public readonly byte[] Bytes;
        private int[] _cityIndexPrepared;

        public GeobaseHeaderMarshal Header { get; }

        private Tuple<int, int> HeaderFromTo => Tuple.Create(0, (int)Header.OffsetRanges);
        private Tuple<int, int> RangesFromTo => Tuple.Create((int)Header.OffsetRanges, (int) Header.OffsetLocations);
        private Tuple<int, int> LocationsFromTo => Tuple.Create((int)Header.OffsetLocations, (int) Header.OffsetCities);
        private Tuple<int, int> CitiesFromTo => Tuple.Create((int)Header.OffsetCities, Bytes.Length);

        private int Records => Header.Records;

        public int HeaderLengh { get; }
        public int RangesLengh { get; }
        public int LocationLengh { get; }
        public int CityIndexLengh { get; }

        private int CityOffsetInLocation { get; }
        private int LocationCityBytesLenght { get;}

        public GeobaseEngineCombined(string fileName)
        {
            var stopwatchAll = Stopwatch.StartNew();

            // init sizes
            HeaderLengh = Extentions.GetBytesLength(typeof(GeobaseHeaderDirrect));
            RangesLengh = Extentions.GetBytesLength(typeof(GeobaseIpRangeDirrect));
            LocationLengh = Extentions.GetBytesLength(typeof(GeobaseLocationDirrect));
            CityIndexLengh = Extentions.GetBytesLength(typeof(GeobaseCityIndexDirrect));

            CityOffsetInLocation = Extentions.GetBytesLength(typeof(GeobaseLocationDirrect), "Country")
                                   + Extentions.GetBytesLength(typeof(GeobaseLocationDirrect), "Region")
                                   + Extentions.GetBytesLength(typeof(GeobaseLocationDirrect), "Postal");

            LocationCityBytesLenght = Extentions.GetBytesLength(typeof(GeobaseLocationDirrect), "City");

            // init data
            Bytes = File.ReadAllBytes(fileName);

            Header = ByteHelper.BytesToStruct<GeobaseHeaderMarshal>(ByteHelper.GetBytes(Bytes, 0, HeaderLengh));
            var cityIndexesOffsets = ByteHelper.BytesToStruct<GeobaseLocationCityIndexesCombined>(ByteHelper.GetBytes(Bytes, (int)CitiesFromTo.Item1, (int)(CitiesFromTo.Item2 -CitiesFromTo.Item1)));
            _cityIndexPrepared = new int[Records];
            for (int i = 0; i < Records; i++)
            {
                _cityIndexPrepared[i] = (int)(cityIndexesOffsets.CityIndexes[i].LocationOffset / LocationLengh);
            }

            stopwatchAll.Stop();

            Debug.WriteLine("GeobaseEngineCombined, DB loading time: " + stopwatchAll.Elapsed.TotalMilliseconds + " ms");
        }

        private GeobaseLocationView GetLocation(byte[] bytes, int offset)
        {
            return TransformationHelper.GetLocationView(
                    ByteHelper.BytesToStruct<GeobaseLocationMarshal>(ByteHelper.GetBytes(bytes, offset, LocationLengh))
            );
        }

        public GeobaseLocationView FindLocationByIp(string ipString)
        {
            var stopwatch = Stopwatch.StartNew();

            var ipUint = IpHelper.IpStringToUint(ipString);
            int locactionIndex = -1;

            // binary search
            int min = 0;
            int max = (RangesFromTo.Item2 - RangesFromTo.Item1) / RangesLengh - 1;  // equal to Records

            while (min <= max)
            {  
                int mid = (min + max) / 2;

                var offset = RangesFromTo.Item1 + mid * RangesLengh;
                var ipFrom = ByteHelper.GetUInt(Bytes, ref offset, true);
                var ipTo = ByteHelper.GetUInt(Bytes, ref offset, true);

                if (ipFrom <= ipUint && ipTo >= ipUint)
                {  
                    locactionIndex = (int)ByteHelper.GetUInt(Bytes, ref offset, true);
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
                Debug.WriteLine("GeobaseEngineCombined, FindLocationByIp, ip = " + ipString + " time: " + stopwatch.Elapsed.TotalMilliseconds + " ms. Result: NOT FOUND");
                return null;
            }

            var locationOffset = LocationsFromTo.Item1 + LocationLengh * locactionIndex;

            Debug.WriteLine("GeobaseEngineCombined, FindLocationByIp, ip = " + ipString + " time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");

            return GetLocation(Bytes, (int)locationOffset);
        }


        public IEnumerable<GeobaseLocationView> FindLocationsByCity(string city)
        {
            var stopwatch = Stopwatch.StartNew();

            // binary search
            int min = 0;
            int max = _cityIndexPrepared.Length - 1;
            bool found = false;
            while (min <= max)
            {
                int mid = (min + max) / 2;

                var locationOffset = LocationsFromTo.Item1 + _cityIndexPrepared[mid] * LocationLengh;
                var locationCityOffset = (locationOffset + CityOffsetInLocation);

                var locationCity = ByteHelper.BytesToStruct<GeobaseLocationCityCombined>(
                    ByteHelper.GetBytes(Bytes, locationCityOffset, LocationCityBytesLenght))
                    .City;

                var comparisonResult = string.CompareOrdinal(city, locationCity);

                if ( comparisonResult == 0)
                {
                    yield return GetLocation(Bytes, locationOffset);

                    var previousCity = city;
                    var pointer = mid - 1;

                    while (pointer >= 0)
                    {
                        locationOffset = LocationsFromTo.Item1 + _cityIndexPrepared[pointer] * LocationLengh;
                        locationCityOffset = (locationOffset + CityOffsetInLocation);
                        previousCity = ByteHelper.BytesToStruct<GeobaseLocationCityCombined>(
                                ByteHelper.GetBytes(Bytes, locationCityOffset, LocationCityBytesLenght))
                            .City;

                        if (string.CompareOrdinal(city, previousCity) == 0)
                        {
                            yield return GetLocation(Bytes, locationOffset);
                        }
                        else
                        {
                            break;
                        }
                        pointer --;
                    }


                    var nextCity = city;
                    pointer = mid + 1;

                    while (pointer < _cityIndexPrepared.Length)
                    {
                        locationOffset = LocationsFromTo.Item1 + _cityIndexPrepared[pointer] * LocationLengh;
                        locationCityOffset = (locationOffset + CityOffsetInLocation);
                        nextCity = ByteHelper.BytesToStruct<GeobaseLocationCityCombined>(
                                ByteHelper.GetBytes(Bytes, locationCityOffset, LocationCityBytesLenght))
                            .City;
                        if (string.CompareOrdinal(city, nextCity) == 0)
                        {
                            yield return GetLocation(Bytes, locationOffset);
                        }
                        else
                        {
                            break;
                        }
                        pointer ++;
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
                Debug.WriteLine("GeobaseEngineCombined, FindLocationsByCity, city = " + city + ", time: " + stopwatch.Elapsed.TotalMilliseconds + " ms. Result: NOT FOUND");
            }

            Debug.WriteLine("GeobaseEngineCombined, FindLocationsByCity, city = " + city + ", time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
        }
    }
}
