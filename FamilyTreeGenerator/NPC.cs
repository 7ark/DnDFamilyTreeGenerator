using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyGenerator
{

    public static class Utility
    {
        public enum NameType { LastName, FirstNameMale, FirstNameFemale }

        public static string GenerateName(Random rand, Race race, NameType nameType)
        {
            string name = "";
            int numberOfSyllablesInFirstName = 0;
            int numberOfSyllablesInTitleName = 0;
            switch (nameType)
            {
                case NameType.LastName:
                    numberOfSyllablesInFirstName = rand.Next(race.LastNameMinSyllableCount, race.LastNameMaxSyllableCount + 1);
                    break;
                case NameType.FirstNameMale:
                    numberOfSyllablesInFirstName = rand.Next(race.MaleFirstNameMinSyllableCount, race.MaleFirstNameMaxSyllableCount + 1);
                    numberOfSyllablesInTitleName = rand.Next(race.TitleNameMinSyllableCount, race.TitleNameMaxSyllableCount + 1);
                    break;
                case NameType.FirstNameFemale:
                    numberOfSyllablesInFirstName = rand.Next(race.FemaleFirstNameMinSyllableCount, race.FemaleFirstNameMaxSyllableCount + 1);
                    numberOfSyllablesInTitleName = rand.Next(race.TitleNameMinSyllableCount, race.TitleNameMaxSyllableCount + 1);
                    break;
            }

            string titleName = string.Empty;
            if(race.hasTitleName)
            {
                for (int i = 0; i < numberOfSyllablesInTitleName; i++)
                {
                    string newSyllable = string.Empty;

                    while(true)
                    {
                        newSyllable = GetWeightedString(rand, race.TitleNameSyllables);

                        if(newSyllable == "-")
                        {
                            if(titleName != string.Empty && i != numberOfSyllablesInTitleName - 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    titleName += newSyllable;
                }
            }

            string firstName = string.Empty;
            for (int i = 0; i < numberOfSyllablesInFirstName; i++)
            {
                string newSyllable = string.Empty;

                while(true)
                {
                    switch (nameType)
                    {
                        case NameType.LastName:
                            newSyllable = GetWeightedString(rand, race.LastNameSyllables);
                            break;
                        case NameType.FirstNameMale:
                            newSyllable = GetWeightedString(rand, race.MaleFirstNameSyllables);
                            break;
                        case NameType.FirstNameFemale:
                            newSyllable = GetWeightedString(rand, race.FemaleFirstNameSyllables);
                            break;
                    }

                    if (nameType != NameType.LastName && i != numberOfSyllablesInFirstName - 1 && race.inBetweenWord != string.Empty)
                    {
                        newSyllable += race.inBetweenWord;
                    }

                    if (newSyllable == "-")
                    {
                        if (firstName != string.Empty && i != numberOfSyllablesInFirstName - 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }


                firstName += newSyllable;
            }

            if(titleName != string.Empty)
            {
                name = titleName + " ";
            }

            name += firstName;

            string refinedName = string.Empty; ;
            char last = ' ';
            for (int i = 0; i < name.Length; i++)
            {
                if(last == ' ' || last == '-')
                {
                    refinedName += name[i].ToString().ToUpper();
                }
                else
                {
                    refinedName += name[i];
                }

                last = name[i];
            }

            return refinedName;
        }

        public static string GetWeightedString(Random rand, List<KeyValuePair<int, string>> table)
        {
            int total = 0;
            foreach (var pair in table)
            {
                total += pair.Key;
            }

            int selected = rand.Next(0, total + 1);

            foreach (var pair in table)
            {
                selected -= pair.Key;

                if (selected <= 0)
                {
                    return pair.Value;
                }
            }

            return "invalid";
        }
    }

    public enum Birthplace { Home = 50, HomeOfFamilyFriend = 55, HomeOfHealer = 63, Carriage = 65, BarnShed = 68, Cave = 70, Field = 72, Forest = 74, Temple = 77, Battlefield = 78, AlleyStreet = 80, BrothalTavernInn = 82, CastleTowerPalace = 84, Sewer = 85, AmongDifferentRace = 88, Boat = 91, Prison = 93, SageLab = 95, FeyWild = 96, Shadowfell = 97, AstralEtherealPlane = 98, InnerPlane = 99, OuterPlane = 100 }
    public enum Upbringing { None = 1, Instiution = 2, Temple = 3, Orphanage = 5, Guardian = 7, ExtendedFamily = 15, Grandparents = 25, AdoptiveParents = 35, SingleFather = 55, SingleMother = 75, MotherAndFather = 100 }
    public enum AbsentParent { NotAbsent, ParentDied, ParentImprisoned, Abandoned, DisappearedToUnknown }
    public enum Childhood { HauntedByChildhood = 3, SpentAlone = 5, StrangeFewFriends = 8, FewCloseFriends = 12, SeveralFriendsHappy = 15, LovedPeople = 17, PracticallyFamous = 18 }
    public enum Background { Acolyte, Charlatan, Criminal, Entertainer, FolkHero, GuildArtisan, Hermit, Noble, Outlander, Sage, Sailor, Soldier, Urchin }

    public struct NPCData
    {
        public FantasyDate CurrentDate;
        public Race Race;
        public string CityOfOrigin;
        public string SurName;
    }

    public class NPC
    {
        public static List<string> FamilyScript = new List<string>();

        public string Name;
        public FantasyDate BirthDate;
        public FantasyDate DeathDate;
        public int Age;
        public bool Dead;
        public bool Male;
        public string Race;
        public bool KnewParents;
        public Birthplace BirthPlace;
        public List<NPC> Siblings = new List<NPC>();
        public List<NPC> Children = new List<NPC>();
        public NPC Father;
        public NPC Mother;
        public Upbringing Family;
        public AbsentParent AbsentParentReason;
        public Childhood Childhood;
        public Background Background;
        public string CityOfOrigin;
        public List<LifeEvent> LifeEvents = new List<LifeEvent>();

        public string id;

        public void GenerateID()
        {
            id = Guid.NewGuid().ToString("N");
            id = id.Substring(0, 5);
        }

        public void GenerateLife(Random rand, NPCData data, FantasyDate referenceDate)
        {
            GenerateID();

            BirthDate = referenceDate - rand.Next((int)(data.Race.AverageLifeSpan * 0.2f), (int)(data.Race.AverageLifeSpan * 0.8f));
            DeathDate = BirthDate + (data.Race.AverageLifeSpan - (data.Race.AverageLifeSpan / 4)) + rand.Next(1, data.Race.AverageLifeSpan / 2);
            Dead = DeathDate < data.CurrentDate;
            Age = Dead ? DeathDate - BirthDate : data.CurrentDate - BirthDate;
            Male = rand.Next(0, 2) == 0;
            Race = data.Race.Name;
            CityOfOrigin = data.CityOfOrigin;
            KnewParents = rand.Next(1, 101) <= 95;

            int birthPlaceRoll = rand.Next(1, 101);
            List<Birthplace> birthPlaceOptions = new List<Birthplace>((IEnumerable<Birthplace>)System.Enum.GetValues(typeof(Birthplace)));

            for (int i = 0; i < birthPlaceOptions.Count; i++)
            {
                int val = (int)birthPlaceOptions[i];
                if (birthPlaceRoll <= val)
                {
                    BirthPlace = birthPlaceOptions[i];
                    break;
                }
            }

            int upbringingRoll = rand.Next(1, 101);
            List<Upbringing> upbringingOptions = new List<Upbringing>((IEnumerable<Upbringing>)System.Enum.GetValues(typeof(Upbringing)));

            for (int i = 0; i < upbringingOptions.Count; i++)
            {
                int val = (int)upbringingOptions[i];
                if (upbringingRoll <= val)
                {
                    Family = upbringingOptions[i];
                    break;
                }
            }

            Background = (Background)rand.Next(0, System.Enum.GetValues(typeof(Background)).Length);

            if (!KnewParents || Family != Upbringing.MotherAndFather)
            {
                AbsentParentReason = (AbsentParent)rand.Next(1, 5);
            }
            else
            {
                AbsentParentReason = AbsentParent.NotAbsent;
            }

            int childhoodRoll = rand.Next(1, 7) + rand.Next(1, 7) + rand.Next(1, 7);
            List<Childhood> childhoodOptions = new List<Childhood>((IEnumerable<Childhood>)System.Enum.GetValues(typeof(Childhood)));

            for (int i = 0; i < childhoodOptions.Count; i++)
            {
                int val = (int)childhoodOptions[i];
                if (childhoodRoll <= val)
                {
                    Childhood = childhoodOptions[i];
                    break;
                }
            }

            int lifeEvents = 1;
            if (Age > 100)
            {
                lifeEvents = rand.Next(1, 13);
            }
            else if (Age > 90)
            {
                lifeEvents = rand.Next(1, 11);
            }
            else if (Age > 70)
            {
                lifeEvents = rand.Next(1, 9);
            }
            else if (Age > 60)
            {
                lifeEvents = rand.Next(1, 7);
            }
            else if (Age > 21)
            {
                lifeEvents = rand.Next(1, 5);
            }

            List<LifeEventOption> lifeEventOptions = new List<LifeEventOption>((IEnumerable<LifeEventOption>)System.Enum.GetValues(typeof(LifeEventOption)));
            for (int i = 0; i < lifeEvents; i++)
            {
                int lifeEventRoll = rand.Next(1, 101);

                LifeEvent lifeEvent = new LifeEvent();
                for (int j = 0; j < lifeEventOptions.Count; j++)
                {
                    int val = (int)lifeEventOptions[j];
                    if (lifeEventRoll <= val)
                    {
                        lifeEvent.Event = lifeEventOptions[j];
                        break;
                    }
                }

                switch (lifeEvent.Event)
                {
                    case LifeEventOption.Tragedy:
                        break;
                    case LifeEventOption.GoodFortune:
                        break;
                    case LifeEventOption.EnemyOfAdventurer:
                        break;
                    case LifeEventOption.MetSomeoneImportant:
                        break;
                    case LifeEventOption.WentOnAdventure:
                        break;
                    case LifeEventOption.SupernaturalExperience:
                        break;
                    case LifeEventOption.Crime:
                        break;
                    case LifeEventOption.MagicEncounter:
                        break;
                    case LifeEventOption.StrangeEvent:
                        break;
                }

                LifeEvents.Add(lifeEvent);
            }
        }

        public List<NPC> CreateLineage(Random rand, NPCData data, FantasyDate referenceDate, ref Queue<string> lastNameCache, int depth = 0, int generations = 3, bool? isMale = null, bool hasSiblings = true)
        {
            List<NPC> result = new List<NPC>();

            GenerateLife(rand, data, referenceDate);
            if (isMale != null)
            {
                Male = (bool)isMale;
            }
            Name = Utility.GenerateName(rand, data.Race, Male ? Utility.NameType.FirstNameMale : Utility.NameType.FirstNameFemale) + " " + data.SurName;//new Faker().Name.FirstName(Male ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female) + " " + data.SurName;

            result.Add(this);


            int siblingCount = rand.Next(1, 11);
            if (siblingCount >= 9)
            {
                siblingCount = rand.Next(1, 9) + 3;
            }
            else if (siblingCount >= 7)
            {
                siblingCount = rand.Next(1, 7) + 2;
            }
            else if (siblingCount >= 5)
            {
                siblingCount = rand.Next(1, 5) + 1;
            }
            else if (siblingCount >= 3)
            {
                siblingCount = rand.Next(1, 4);
            }
            else if (siblingCount >= 0)
            {
                siblingCount = 0;
            }

            if(!hasSiblings)
            {
                siblingCount = 0;
            }

            for (int i = 0; i < siblingCount; i++)
            {
                NPC sibling = new NPC();
                sibling.GenerateLife(rand, data, referenceDate);
                sibling.Name = Utility.GenerateName(rand, data.Race, sibling.Male ? Utility.NameType.FirstNameMale : Utility.NameType.FirstNameFemale) + " " + data.SurName;
                sibling.Family = Family;
                sibling.AbsentParentReason = AbsentParentReason;

                Siblings.Add(sibling);
                result.Add(sibling);
            }

            FantasyDate youngest = BirthDate;

            for (int i = 0; i < Siblings.Count; i++)
            {
                for (int j = 0; j < Siblings.Count; j++)
                {
                    if (i != j)
                    {
                        Siblings[i].Siblings.Add(Siblings[j]);
                    }
                }
                Siblings[i].Siblings.Add(this);

                if (Siblings[i].BirthDate < youngest)
                {
                    youngest = Siblings[i].BirthDate;
                }
            }

            if (depth < generations)
            {
                NPC father = new NPC();
                result.AddRange(father.CreateLineage(rand, data, youngest, ref lastNameCache, depth + 1, generations: generations, isMale: true));

                NPC mother = new NPC();
                string surName = string.Empty;
                if (data.Race.hasLastName && lastNameCache.Count > 0)
                {
                    surName = lastNameCache.Dequeue();
                }
                data.SurName = surName;
                result.AddRange(mother.CreateLineage(rand, data, youngest, ref lastNameCache, depth + 1, generations: generations, isMale: false));

                Father = father;
                Mother = mother;
                for (int i = 0; i < Siblings.Count; i++)
                {
                    Siblings[i].Father = father;
                    Siblings[i].Mother = mother;

                    Siblings[i].GenerateFamilyScript();
                }

                Father.Children.Add(this);
                Mother.Children.Add(this);
                Father.Children.AddRange(Siblings);
                Mother.Children.AddRange(Siblings);
            }

            GenerateFamilyScript();

            return result;
        }

        private void GenerateFamilyScript()
        {
            string fs = "i" + id + "\tp" + Name.Split(' ')[0];
            if (Name.Contains(" "))
            {
                string last = string.Empty;
                string[] tempSplit = Name.Split(' ');
                for (int i = 1; i < tempSplit.Length; i++)
                {
                    last += tempSplit[i];
                    if(i != tempSplit.Length - 1)
                    {
                        last += " ";
                    }
                }
                fs += "\t" + "l" + last;
            }
            if (Father != null)
            {
                fs += "\tf" + Father.id + "\tm" + Mother.id;
            }
            fs += "\tb" + BirthDate.GetYearUniversal().ToString("0000");
            if (Dead)
            {
                fs += "\td" + DeathDate.GetYearUniversal().ToString("0000");
                fs += "\tz1";
            }
            fs += "\tg" + (Male ? "m" : "f");

            if (LifeEvents.Count > 0)
            {
                fs += "\tA";
                for (int i = 0; i < LifeEvents.Count; i++)
                {
                    fs += LifeEvents[i].Event + LifeEvents[i].ExtraFlavor;
                    if (i != LifeEvents.Count - 1)
                    {
                        fs += ", ";
                    }
                }
            }

            fs += "\tj" + Background;
            fs += "\tv" + BirthPlace + ", " + CityOfOrigin;

            FamilyScript.Add(fs);
        }

        public override string ToString()
        {
            string result = Name + "\nLifespan " + BirthDate.GetYearInAge() + BirthDate.GetSymbol() + " - " + (Dead ? DeathDate.GetYearInAge().ToString() : "???") + DeathDate.GetSymbol() + " " + (Male ? "Male" : "Female") + " " + Race + "\n" +
                   "Age " + Age + "\n\n" +
                   "Father: " + (Father == null ? "Unknown" : Father.Name) + "\n" +
                   "Mother: " + (Mother == null ? "Unknown" : Mother.Name) + "\n" +
                   Siblings.Count + " Siblings" + (Siblings.Count > 0 ? " (" : string.Empty);

            for (int i = 0; i < Siblings.Count; i++)
            {
                result += Siblings[i].Name;
                if (i < Siblings.Count - 1)
                {
                    result += ", ";
                }
                else
                {
                    result += ")";
                }
            }

            result += "\n\n" + (KnewParents ? "Knew their parents" : "Did not know their parents") + ", was raised by " + Family + (AbsentParentReason == AbsentParent.NotAbsent ? "." : " with an absent parent because of " + AbsentParentReason);
            result += "\nBorn in " + BirthPlace + " near or connected to " + CityOfOrigin;
            result += "\nChildhood: " + Childhood;

            if (Children.Count > 0)
            {
                result += "\n\nChildren: (";

                for (int i = 0; i < Children.Count; i++)
                {
                    result += Children[i].Name;

                    if (i < Children.Count - 1)
                    {
                        result += ", ";
                    }
                    else
                    {
                        result += ")";
                    }
                }
            }
            else
            {
                result += "\n\nNo Children";
            }

            result += "\n\nLife Events (" + LifeEvents.Count + "):";
            for (int i = 0; i < LifeEvents.Count; i++)
            {
                result += "\n\t-" + LifeEvents[i].Event.ToString() + " " + LifeEvents[i].ExtraFlavor;
            }

            return result;
        }
    }

    public enum LifeEventOption { Tragedy = 10, GoodFortune = 20, Love = 30, EnemyOfAdventurer = 40, FriendOfAdventurer = 50, Working = 70, MetSomeoneImportant = 75, WentOnAdventure = 80, SupernaturalExperience = 85, FoughtInBattle = 90, Crime = 95, MagicEncounter = 99, StrangeEvent = 100 }

    public struct LifeEvent
    {
        public LifeEventOption Event;
        public string ExtraFlavor;
    }

    public struct Race
    {
        public string Name;
        public int AverageLifeSpan;

        //Last Names
        public bool hasLastName;
        public List<KeyValuePair<int, string>> LastNameSyllables;
        public int LastNameMinSyllableCount;
        public int LastNameMaxSyllableCount;

        public bool hasTitleName;
        public List<KeyValuePair<int, string>> TitleNameSyllables;
        public int TitleNameMinSyllableCount;
        public int TitleNameMaxSyllableCount;

        //Male First Names
        public List<KeyValuePair<int, string>> MaleFirstNameSyllables;
        public int MaleFirstNameMinSyllableCount;
        public int MaleFirstNameMaxSyllableCount;

        //Male First Names
        public List<KeyValuePair<int, string>> FemaleFirstNameSyllables;
        public int FemaleFirstNameMinSyllableCount;
        public int FemaleFirstNameMaxSyllableCount;

        public string inBetweenWord;
    }
}
