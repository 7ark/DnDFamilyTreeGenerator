using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FamilyGenerator
{
    public enum Age { Generic_Age }
    public struct FantasyDate : IComparable<FantasyDate>
    {
        private Dictionary<Age, int> ageLengths;

        public Age CurrentAge
        {
            get
            {
                Age temp = Age.Generic_Age;
                int tempYear = year;
                while(tempYear > 0)
                {
                    tempYear -= ageLengths[temp];
                    if(tempYear <= 0)
                    {
                        return temp;
                    }
                    temp++;
                }
                return Age.Generic_Age;
            }
        }
        private int year;

        public FantasyDate(Age age, int year)
        {
            ageLengths = new Dictionary<Age, int>
            {
                { Age.Generic_Age, 10000 }
            };

            int extraTime = 0;
            Age temp = Age.Generic_Age;
            while(temp != age)
            {
                extraTime += ageLengths[temp];
                temp++;
            }
            this.year = year + extraTime;
        }

        public int GetYearInAge()
        {
            Age temp = Age.Generic_Age;
            int tempYear = year;
            while (tempYear > 0)
            {
                if (tempYear < ageLengths[temp])
                {
                    return tempYear;
                }
                tempYear -= ageLengths[temp];
                temp++;
            }

            return year;
        }

        public int GetYearUniversal()
        {
            return year;
        }

        public string GetSymbol()
        {
            string[] split = CurrentAge.ToString().Split('_');
            return new string(new[] { split[0][0], split[1][0] });
        }

        public int CompareTo(FantasyDate obj)
        {
            return year.CompareTo(obj.year);
        }

        public static implicit operator int(FantasyDate d)
        {
            return d.GetYearInAge();
        }

        public static FantasyDate operator +(FantasyDate a, int time)
        {
            a.year += time;

            return a;
        }

        public static FantasyDate operator -(FantasyDate a, int time)
        {
            a.year -= time;

            return a;
        }

        public static bool operator <(FantasyDate a, FantasyDate b)
        {
            return a.year < b.year;
        }

        public static bool operator >(FantasyDate a, FantasyDate b)
        {
            return a.year > b.year;
        }
    }

    public class FamilyGenerator
    {
        public static NPC Generate(string primaryPath, int generations, string[] familyLines, Race currentRace, FantasyDate currentDate, string city, string specificFamilyName = "", bool firstHasSiblings = true, string extraText = "")
        {
            NPC resultNpc = null;
            string path = primaryPath + @"\Families\" + city + @"\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += currentRace.Name;

            Random rand = new Random();
            List<NPC> allNpcs = new List<NPC>();


            Queue<string> lastNamesCache = new Queue<string>();
            if(currentRace.hasLastName)
            {
                int namesToMake = (familyLines.Length * generations) * 4000;
                Console.WriteLine("Creating " + namesToMake + " unique last names");
                int safety = 0;
                while (lastNamesCache.Count < namesToMake)
                {
                    string lastName = Utility.GenerateName(rand, currentRace, Utility.NameType.LastName);
                    if (!lastNamesCache.Contains(lastName))
                    {
                        lastNamesCache.Enqueue(lastName);
                        safety = 0;
                    }
                    safety++;

                    if (safety > 5000)
                    {
                        Console.WriteLine("Could only generate " + lastNamesCache.Count + " unique last names before running out of variations.");
                        break;
                    }
                }
            }

            int livingPeople = 0;

            for (int f = 0; f < familyLines.Length; f++)
            {
                List<NPC> living = new List<NPC>();
                while (true)
                {
                    living.Clear();
                    allNpcs.Clear();
                    while (allNpcs.Count == 0)
                    {
                        NPC.FamilyScript.Clear();
                        Console.WriteLine("Creating NPCs");
                        NPC npc = new NPC();
                        allNpcs.AddRange(npc.CreateLineage(rand, new NPCData()
                        {
                            CurrentDate = currentDate,
                            Race = currentRace,
                            SurName = specificFamilyName != string.Empty ? specificFamilyName : currentRace.hasLastName ? lastNamesCache.Dequeue() : string.Empty,
                            CityOfOrigin = city
                        },
                        currentDate, ref lastNamesCache, generations: generations, hasSiblings: firstHasSiblings));

                        allNpcs.Sort((x, y) => { return y.BirthDate.CompareTo(x.BirthDate); });

                        for (int i = 0; i < allNpcs.Count; i++)
                        {
                            for (int j = 0; j < allNpcs.Count; j++)
                            {
                                if (i != j)
                                {
                                    NPC one = allNpcs[i];
                                    NPC two = allNpcs[j];
                                    if (one.Name == two.Name && currentRace.hasLastName && Math.Abs(one.BirthDate.GetYearUniversal() - two.BirthDate.GetYearUniversal()) < currentRace.AverageLifeSpan * 0.1f)
                                    {
                                        allNpcs.Clear();
                                        Console.WriteLine("Found duplicate, regenerating");
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < allNpcs.Count; i++)
                    {
                        if (!allNpcs[i].Dead)
                        {
                            living.Add(allNpcs[i]);
                            if(allNpcs[i].Age > currentRace.AverageLifeSpan * 0.3f || resultNpc == null)
                                resultNpc = allNpcs[i];
                        }
                    }

                    if(living.Count >= 5 || !firstHasSiblings)
                    {
                        livingPeople += living.Count;
                        break;
                    }
                }

                Console.WriteLine("There are " + living.Count + " living in " + familyLines[f] + ".");


                Console.WriteLine("Writing NPCs");
                using (FileStream stream = new FileStream(path + "_AllGenerationInfo_" + familyLines[f] + ".txt", FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        for (int i = 0; i < allNpcs.Count; i++)
                        {
                            writer.WriteLine(allNpcs[i].ToString());
                            if(allNpcs[i] == resultNpc)
                            {
                                writer.WriteLine(extraText);
                            }
                            writer.WriteLine("\n" + new string('-', 20) + "\n");
                        }
                    }
                }
                using (FileStream stream = new FileStream(path + "_FamilyScript_" + familyLines[f] + ".txt", FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        for (int j = 0; j < NPC.FamilyScript.Count; j++)
                        {
                            writer.WriteLine(NPC.FamilyScript[j]);
                        }

                    }
                }
            }

            Console.WriteLine("Done with " + livingPeople + " citizens");

            return resultNpc;
        }
    }
}
