using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Engine.Attribute;
using Engine.Geobase.Dirrect.DataDescription;
using Engine.Helpers;

namespace Engine.Geobase.Dirrect
{
    public class GeobaseEngineDirrect: IGeobaseEngine
    {
        public readonly byte[] Bytes;

        public GeobaseHeaderDirrect Header { get; }

        private Tuple<int, int> HeaderFromTo => Tuple.Create(0, (int)Header.OffsetRanges);
        private Tuple<int, int> RangesFromTo => Tuple.Create((int)Header.OffsetRanges, (int)Header.OffsetLocations);
        private Tuple<int, int> LocationsFromTo => Tuple.Create((int)Header.OffsetLocations, (int)Header.OffsetCities);
        private Tuple<int, int> CitiesFromTo => Tuple.Create((int)Header.OffsetCities, Bytes.Length);

        private int Records => Header.Records;

        public int HeaderLengh { get; }
        public int RangesLengh { get; }
        public int LocationLengh { get; }
        public int CityIndexLengh { get; }

        private int CityOffsetInLocation { get; }
        private int LocationCityBytesLenght { get;}

        public GeobaseEngineDirrect(string fileName)
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

            int offset = 0;
            Header = new GeobaseHeaderDirrect
            {
                Version = ByteHelper.GetInt(Bytes, ref offset, true),
                Name = ByteHelper.GetString(Bytes, 32, ref offset, true),
                Timestamp = ByteHelper.GetULong(Bytes, ref offset, true),
                Records = ByteHelper.GetInt(Bytes, ref offset, true),
                OffsetRanges = ByteHelper.GetUInt(Bytes, ref offset, true),
                OffsetCities = ByteHelper.GetUInt(Bytes, ref offset, true),
                OffsetLocations = ByteHelper.GetUInt(Bytes, ref offset, true)
            };

            stopwatchAll.Stop();

            Debug.WriteLine("GeobaseEngineDirrect, DB loading time: " + stopwatchAll.Elapsed.TotalMilliseconds + " ms");
        }

        private GeobaseLocationView GetLocation(byte[] bytes, int offset)
        {
            return ByteHelper.GetLocation(Bytes, (int) offset);
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
                Debug.WriteLine("GeobaseEngineDirrect, FindLocationByIp, ip = " + ipString + " time: " + stopwatch.Elapsed.TotalMilliseconds + " ms. Result: NOT FOUND");
                return null;
            }

            var locationOffset = LocationsFromTo.Item1 + LocationLengh * locactionIndex;

            Debug.WriteLine("GeobaseEngineDirrect, FindLocationByIp, ip = " + ipString + " time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");

            return GetLocation(Bytes, (int)locationOffset);
        }


        public IEnumerable<GeobaseLocationView> FindLocationsByCity(string city)
        {
            var stopwatch = Stopwatch.StartNew();
            var cityBytes = Encoding.ASCII.GetBytes(city);

            // binary search
            int min = 0;
            int max = (CitiesFromTo.Item2 - CitiesFromTo.Item1) / CityIndexLengh - 1;
            bool found = false;
            while (min <= max)
            {
                int mid = (min + max) / 2;

                var offsetCityPrimary = CitiesFromTo.Item1 + mid * CityIndexLengh;
                
                var locationOffset = (int)(LocationsFromTo.Item1 + ByteHelper.GetUInt(Bytes, ref offsetCityPrimary, false));
                var locationCityOffset = locationOffset + CityOffsetInLocation;

                var locationCityBytes = new ArraySegment<byte>(Bytes, locationCityOffset, LocationCityBytesLenght);

                var comparisonResult = ByteHelper.CompareStringsAsBytes(cityBytes, locationCityBytes);

                if ( comparisonResult == 0)
                {
                    yield return GetLocation(Bytes, locationOffset);

                    var previousCity = new ArraySegment<byte>(cityBytes);
                    var pointer = offsetCityPrimary - CityIndexLengh;

                    while (pointer >= CitiesFromTo.Item1)
                    {
                        locationOffset = (int)(LocationsFromTo.Item1 + ByteHelper.GetUInt(Bytes, ref pointer, false));
                        locationCityOffset = (locationOffset + CityOffsetInLocation);
                        previousCity = new ArraySegment<byte>(Bytes, locationCityOffset, LocationCityBytesLenght);
                        if (ByteHelper.CompareStringsAsBytes(cityBytes, previousCity) == 0)
                        {
                            yield return GetLocation(Bytes, locationOffset);
                        }
                        else
                        {
                            break;
                        }
                        pointer = pointer - CityIndexLengh;
                    }

                    var nextCity = new ArraySegment<byte>(cityBytes);
                    pointer = offsetCityPrimary + CityIndexLengh;

                    while (pointer < CitiesFromTo.Item2)
                    {
                        locationOffset = (int)(LocationsFromTo.Item1 + ByteHelper.GetUInt(Bytes, ref pointer, false));
                        locationCityOffset = (locationOffset + CityOffsetInLocation);
                        nextCity = new ArraySegment<byte>(Bytes, locationCityOffset, LocationCityBytesLenght);
                        if (ByteHelper.CompareStringsAsBytes(cityBytes, nextCity) == 0)
                        {
                            yield return GetLocation(Bytes, locationOffset);
                        }
                        else
                        {
                            break;
                        }
                        pointer = pointer + CityIndexLengh;
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
                Debug.WriteLine("GeobaseEngineDirrect, FindLocationsByCity, city = " + city + ", time: " + stopwatch.Elapsed.TotalMilliseconds + " ms. Result: NOT FOUND");
            }

            Debug.WriteLine("GeobaseEngineDirrect, FindLocationsByCity, city = " + city + ", time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
        }
    }
}
