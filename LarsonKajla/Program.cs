using System;
using System.Collections.Generic;
using System.Text;

namespace LarsonKajla
{
    //algorithm source https://dl.acm.org/doi/10.1145/358105.358193

    static class Const
    {
        public const int blockingFactor = 5;
        public const int pages = 7;
        public static int recordCounter = 0;
    }

    /// <summary>
    /// Data class, position of car in time
    /// </summary>
    class CarRecord
    {
        public int id;
        public int time;
        public string spz;
        public double lat;
        public double lon;
        public int S;

        public CarRecord(int time, string spz, double lat, double lon)
        {
            this.id = Const.recordCounter;
            Const.recordCounter++;
            this.time = time;
            this.spz = spz;
            this.lat = lat;
            this.lon = lon;
        }

        public override string ToString()
        {
            return $"{id}\t{time}\t{spz} \t{lat}\t{lon}\t{S}\n";
        }
    }

    /// <summary>
    /// Standard page containing car records
    /// </summary>
    class Page
    {
        public int separator;
        public int recordCount;
        public CarRecord[] records;

        public Page()
        {
            this.separator = Const.pages;                       //max possible separator
            this.recordCount = 0;
            records = new CarRecord[Const.blockingFactor];      //one page can only fit so much records
        }

        public bool InsertHere(CarRecord rec)
        {
            if (recordCount >= Const.blockingFactor)
            {
                return false;
            }
            else
            {
                records[recordCount] = rec;
                recordCount++;
                return true;
            }
        }

        public List<CarRecord> Empty(CarRecord newRec)
        {
            List<CarRecord> ret = new List<CarRecord>();
            ret.Add(newRec);
            int maxS = newRec.S;
            for (int i = 0; i < Const.blockingFactor; i++)
            {
                ret.Add(records[i]);
                if (records[i].S > maxS)
                {
                    maxS = records[i].S;
                }
            }
            separator = maxS;
            records = new CarRecord[Const.blockingFactor];
            recordCount = 0;
            return ret;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"Sep: {separator}, Cou: {recordCount}\n");
            sb.Append($"id\ttime\t\tspz\t\tlat\t\tlon\t\tS\n");
            foreach (CarRecord cr in records)
            {
                sb.Append(cr);
            }
            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //in my demo I have 20 records, with blocking factor 5 and 7 pages small number of collision will happen
            List<CarRecord> records = new List<CarRecord>();
            //seed test data
            Seed(records);

            Page[] pages = new Page[Const.pages];
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i] = new Page();
            }

            foreach (CarRecord cr in records)
            {
                Insert(pages, cr);
            }

            foreach (Page p in pages)
            {
                Console.WriteLine(p);
            }
        }

        private static void Insert(Page[] pages, CarRecord cr)
        {
            CarRecord rec = cr;
            int key = rec.time;
            bool inserted = false;
            for (int i = 0; i < Const.pages; i++)
            {
                int h = H_i(key, i);
                int s = S_i(key, i);
                if (pages[h].separator > s)
                {
                    rec.S = s;
                    bool success = pages[h].InsertHere(rec);
                    if (!success)
                    {
                        List<CarRecord> reassign = pages[h].Empty(rec);    //empty page
                        foreach (CarRecord subcr in reassign)
                        {
                            Insert(pages, subcr);
                        }
                        Console.WriteLine("Page " + h + " was full");
                    }
                    inserted = true;
                    break;
                }
            }
            if (!inserted) Console.WriteLine("error - no space for insert, increse number of pages ok blocking factor");
        }

        static int H_i(int k, int i)
        {
            return (k + i) % Const.pages;
        }

        static int S_i(int k, int i)
        {
            return (k >> i) % Const.blockingFactor;
        }

        private static void Seed(List<CarRecord> records)
        {
            records.Add(new CarRecord(1604332288, "2A72975", 14.311612, 49.64745));
            records.Add(new CarRecord(1604338678, "2T95071", 16.394138, 50.62306));
            records.Add(new CarRecord(1604345163, "2E94201", 16.140652, 49.28277));
            records.Add(new CarRecord(1604354139, "2H688213", 14.55049, 50.705963));
            records.Add(new CarRecord(1604364454, "1C120351", 13.97618, 49.575512));
            records.Add(new CarRecord(1604373295, "4C126150", 14.860389, 50.363697));
            records.Add(new CarRecord(1604381070, "6Z997482", 14.226425, 50.190014));
            records.Add(new CarRecord(1604391177, "9C14659", 13.006349, 50.409115));
            records.Add(new CarRecord(1604397532, "6T962376", 14.857989, 49.07316));
            records.Add(new CarRecord(1604405689, "3T647430", 15.206197, 49.44672));
            records.Add(new CarRecord(1604412045, "2U049749", 15.9728365, 50.14784));
            records.Add(new CarRecord(1604417789, "8T69593", 16.640383, 50.12476));
            records.Add(new CarRecord(1604426181, "4C115311", 15.234937, 50.63903));
            records.Add(new CarRecord(1604435979, "8E367555", 13.851797, 49.54405));
            records.Add(new CarRecord(1604445433, "6Z45583", 15.20969, 49.596264));
            records.Add(new CarRecord(1604454613, "9L68654", 15.26164, 49.096195));
            records.Add(new CarRecord(1604459709, "1L13639", 13.21952, 49.262524));
            records.Add(new CarRecord(1604469596, "3M785575", 13.989489, 49.967834));
            records.Add(new CarRecord(1604477463, "3U16997", 15.826454, 50.388603));
            records.Add(new CarRecord(1604485873, "8J32195", 13.973006, 50.130928));
        }
    }
}
