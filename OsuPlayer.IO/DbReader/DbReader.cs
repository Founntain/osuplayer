using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OsuPlayer.DbReader
{
    public class DbReader : BinaryReader
    {
        public DbReader(Stream input) : base(input)
        {
        }

        public DbReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public DbReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public static List<MapEntry>? ReadOsuDb(string osuPath)
        {
            var beatmaps = new List<MapEntry>();
            var dbLoc = $"{osuPath}\\osu!.db";

            if (!File.Exists(dbLoc))
            {
                return null;
            }

            using (DbReader reader = new(File.OpenRead(dbLoc)))
            {
                var ver = reader.ReadInt32();
                var flag = ver >= 20160408 && ver < 20191107;
                reader.ReadInt32();
                reader.ReadBoolean();
                reader.ReadInt64();
                reader.ReadString();
                var beatmapcount = reader.ReadInt32();

                for (var i = 1; i < beatmapcount; i++)
                {
                    var _ = reader.BaseStream.Position; //position

                    if (flag)
                        reader.ReadInt32(); //btlen
                    beatmaps.Add(MapEntry.ReadFromReader(reader, ver));
                }

                reader.ReadInt32(); //account rank
            }

            return beatmaps;
        }

        public static List<Collection>? ReadCollections(string osuPath)
        {
            var collections = new List<Collection>();
            var colLoc = $"{osuPath}\\collection.db";

            if (!File.Exists(colLoc))
            {
                return null;
            }
            
            using (DbReader reader = new(File.OpenRead(colLoc)))
            {
                reader.ReadInt32(); //osuVersion
                var num = reader.ReadInt32();

                for (var i = 0; i < num; i++) collections.Add(Collection.ReadFromReader(reader));
            }

            return collections;
        }

        public override string ReadString()
        {
            switch (ReadByte())
            {
                case 0:
                    return string.Empty;
                case 11:
                    return base.ReadString();
                default:
                    throw new Exception();
            }
        }

        public void ReadStarRating()
        {
            var count = ReadInt32();
            ReadBytes(14 * count);
        }

        public DateTime ReadDateTime()
        {
            return new DateTime(ReadInt64(), DateTimeKind.Utc);
        }
    }
}