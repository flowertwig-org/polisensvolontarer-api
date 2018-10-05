using System;
using System.Collections.Generic;
using System.Linq;
using Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Api.Helpers
{
    public class AvailableAssignmentsHelper
    {
        public static List<string> GetTypes()
        {
            // Order MUST remain (stored in user cookies), add more items last
            var types = new List<string>{ 
                "Biträde vid utbildning /möte",
                "Brottsofferstöd",
                "Dagvandring",
                "Figurantuppdrag",
                "Fortbildning",
                "Informationsinsats",
                "Kvällsvandring",
                "Pass / Reception",
                "Volontärmöte",
                "Övrigt",
                "Cykel / Segway",
                "Föreläsning / Temakväll",
                "Nattknappen",
                // NYA
                "Nattvandring",
                "Idrottsevenemang",
                "Demonstration",
                "Trafikuppdrag",
                "Rytteriet",
                "Familje-/ musikevenemang"
            };
            return types;
        }

        public static List<string> GetSpecialTypes()
        {
            var specTypes = new List<string> {
                "EJ går att anmäla intresse till"
            };

            return specTypes;
        }

        public static List<string> GetAreas()
        {
            // Order MUST remain (stored in user cookies), add more items last
            var areas = new List<string>{
                // City
                "Norrmalm",
                "Södermalm",

                // Syd
                "Farsta",
                "Globen",
                "Skärholmen",
                "Botkyrka",
                "Huddinge",
                "Haninge-Nynäshamn",
                "Nacka",
                "Södertälje",

                // Nord
                "Järfälla",
                "Sollentuna",
                "Täby",
                "Norrtälje",
                "Solna",
                "Rinkeby",
                "Vällingby",

                // Centrala uppdrag
                "Operativa enheten",
                "Po Syd",

                // Gotland
                "Gotland",

                // Gränspolisenheten
                "Gränspolisenheten",
                // Övrigt
                "Övrig"
            };
            return areas;
        }

        public static AvailableAssignmentFilterSettings GetFilterSettings(
            string filterAlwaysShowTypes, string filterNeverShowTypes,
            string filterHideWorkDayTypes, string filterHideWeekendTypes,
            string filterNeverShowAreas, string filterAlwaysShowAreas,
            string filterNeverShowSpecTypes)
        {
            var filterSettings = new AvailableAssignmentFilterSettings();

            var typeCount = GetTypes().Count;
            var specialTypeCount = GetSpecialTypes().Count;
            var areaCount = GetAreas().Count;

            if (!string.IsNullOrEmpty(filterAlwaysShowTypes))
            {
                filterSettings.AlwaysShowTypes = filterAlwaysShowTypes.Split(',').Select(str => int.TryParse(str, out int n) ? n : -1).Where(n => n >= 0 && n < typeCount).ToList();
            }
            if (!string.IsNullOrEmpty(filterNeverShowTypes))
            {
                filterSettings.NeverShowTypes = filterNeverShowTypes.Split(',').Select(str => int.TryParse(str, out int n) ? n : -1).Where(n => n >= 0 && n < typeCount).ToList();
            }
            if (!string.IsNullOrEmpty(filterHideWorkDayTypes))
            {
                filterSettings.HideWorkDayTypes = filterHideWorkDayTypes.Split(',').Select(str => int.TryParse(str, out int n) ? n : -1).Where(n => n >= 0 && n < typeCount).ToList();
            }
            if (!string.IsNullOrEmpty(filterHideWeekendTypes))
            {
                filterSettings.HideWorkDayTypes = filterHideWeekendTypes.Split(',').Select(str => int.TryParse(str, out int n) ? n : -1).Where(n => n >= 0 && n < typeCount).ToList();
            }

            if (!string.IsNullOrEmpty(filterNeverShowAreas))
            {
                filterSettings.NeverShowAreas = filterNeverShowAreas.Split(',').Select(str => int.TryParse(str, out int n) ? n : -1).Where(n => n >= 0 && n < areaCount).ToList();
            }
            if (!string.IsNullOrEmpty(filterAlwaysShowAreas))
            {
                filterSettings.AlwaysShowAreas = filterAlwaysShowAreas.Split(',').Select(str => int.TryParse(str, out int n) ? n : -1).Where(n => n >= 0 && n < areaCount).ToList();
            }

            if (!string.IsNullOrEmpty(filterNeverShowSpecTypes))
            {
                filterSettings.NeverShowSpecTypes = filterNeverShowSpecTypes.Split(',').Select(str => int.TryParse(str, out int n) ? n : -1).Where(n => n >= 0 && n < specialTypeCount).ToList();
            }

            return filterSettings;
        }

        public static List<Assignment> FilerItems(List<Assignment> items, AvailableAssignmentFilterSettings filterSettings)
        {
            List<Assignment> filteredItems = new List<Assignment>(items);
            // filters out items that we are not interested in
            var indexesToRemove = new List<int>();
            var itemsMarkedAsRemove = false;

            for (int assignmentIndex = 0; assignmentIndex < items.Count; assignmentIndex++)
            {
                var assignment = items[assignmentIndex];

                var isProtected = false;

                for (int index = 0; index < filterSettings.AlwaysShowTypes.Count; index++)
                {
                    var typeName = GetTypeName(filterSettings.AlwaysShowTypes[index]);
                    if (assignment.Category == typeName)
                    {
                        // Assignment are protected
                        isProtected = true;
                        break;
                    }
                }

                for (int index = 0; index < filterSettings.AlwaysShowAreas.Count; index++)
                {
                    var areaName = GetAreaName(filterSettings.AlwaysShowAreas[index]);
                    if (assignment.Area == areaName)
                    {
                        // Assignment are protected
                        isProtected = true;
                        break;
                    }
                }

                if (isProtected)
                {
                    continue;
                }

                for (int index = 0; index < filterSettings.NeverShowTypes.Count; index++)
                {
                    var typeName = GetTypeName(filterSettings.NeverShowTypes[index]);
                    if (assignment.Category == typeName)
                    {
                        indexesToRemove.Add(assignmentIndex);
                        itemsMarkedAsRemove = true;
                    }
                }


                DateTime date;
                if (DateTime.TryParse(assignment.Date, out date))
                {
                    var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

                    if (!isWeekend)
                    {
                        for (int index = 0; index < filterSettings.HideWorkDayTypes.Count; index++)
                        {
                            var typeName = GetTypeName(filterSettings.HideWorkDayTypes[index]);
                            if (assignment.Category == typeName)
                            {
                                indexesToRemove.Add(assignmentIndex);
                                itemsMarkedAsRemove = true;
                            }
                        }
                    }
                    else
                    {
                        for (int index = 0; index < filterSettings.HideWeekendTypes.Count; index++)
                        {
                            var typeName = GetTypeName(filterSettings.HideWeekendTypes[index]);
                            if (assignment.Category == typeName)
                            {
                                indexesToRemove.Add(assignmentIndex);
                                itemsMarkedAsRemove = true;
                            }
                        }
                    }
                }

                if (!itemsMarkedAsRemove)
                {
                    for (int index = 0; index < filterSettings.NeverShowAreas.Count; index++)
                    {
                        var areaName = GetAreaName(filterSettings.NeverShowAreas[index]);
                        if (assignment.Area == areaName)
                        {
                            indexesToRemove.Add(assignmentIndex);
                            itemsMarkedAsRemove = true;
                        }
                    }
                }

                if (!itemsMarkedAsRemove)
                {
                    for (int index = 0; index < filterSettings.NeverShowSpecTypes.Count; index++)
                    {
                        var typeName = GetSpecTypeName(filterSettings.NeverShowSpecTypes[index]);
                        if (assignment.Category == typeName)
                        {
                            indexesToRemove.Add(assignmentIndex);
                            itemsMarkedAsRemove = true;
                        }
                    }
                }
            }

            if (indexesToRemove.Count > 0)
            {
                indexesToRemove.Reverse();

                for (var removeIndex = 0; removeIndex < indexesToRemove.Count; removeIndex++)
                {
                    var indexToRemove = indexesToRemove[removeIndex];
                    items.RemoveAt(indexToRemove);
                }
            }


            return filteredItems;
        }

        protected static string GetValueFromArray(int index, List<string> arrayToGetValueFrom)
        {
            if (index >= 0 && arrayToGetValueFrom.Count > index)
            {
                return arrayToGetValueFrom[index];
            }
            else
            {
                return "";
            }
        }

        protected static string GetSpecTypeName(int index)
        {
            return GetValueFromArray(index, GetSpecialTypes());
        }

        protected static string GetTypeName(int index)
        {
            return GetValueFromArray(index, GetTypes());
        }

        protected static string GetAreaName(int index)
        {
            return GetValueFromArray(index, GetAreas());
        }

        public static string GroupByDay(Assignment assignment)
        {
            DateTime date;
            if (DateTime.TryParse(assignment.Date, out date))
            {
                return date.ToString("yyyy-MM-dd");
            }
            return "";
        }


        public static List<Assignment> GetAvailableAssignments(HttpContext httpContext)
        {
            var list = new List<Assignment>();
            try
            {
                byte[] data;
                if (httpContext.Session.TryGetValue("AvailableAssignments", out data))
                {
                    var content = System.Text.Encoding.UTF8.GetString(data);
                    var jArray = Newtonsoft.Json.JsonConvert.DeserializeObject(content) as Newtonsoft.Json.Linq.JArray;
                    var array = jArray.ToObject<Assignment[]>();
                    if (array != null)
                    {
                        list.AddRange(array);
                    }
                }
            }
            catch (System.Exception)
            {
                // TODO: Do error handling
            }

            return list;
        }
    }
}
