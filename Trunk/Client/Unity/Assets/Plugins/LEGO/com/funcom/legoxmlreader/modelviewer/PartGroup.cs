using UnityEngine;

namespace com.funcom.legoxmlreader.modelviewer
{
    public class PartGroup
    {
        private Part[] parts;
        private Bounds groupBounds;
        private string name;
        private int i = 0;

        public PartGroup(int size, string name)
        {
            this.name = name;
            parts = new Part[size];
        }

        public void AddPart(Part p)
        {
            if (i >= 0 && i < parts.Length)
            {
                parts[i++] = p;
            }
        }

        public bool ContainsPart(Part otherPart)
        {
            foreach (Part b in parts)
            {
                if (otherPart == b)
                {
                    return true;
                }
            }
            return false;
        }

        public void ComputeBounding()
        {
            groupBounds = parts[0].GetPartBounds();
            for (int i = 1; i < parts.Length; ++i)
            {
                groupBounds.Encapsulate(parts[i].GetPartBounds());
            }
        }

        public Bounds GetBounding()
        {
            return groupBounds;
        }

        public string GetName()
        {
            return name;
        }

        public Part[] GetParts
        {
            get
            {
                return parts;
            }
        }
    }
}