namespace AbstractBinding
{
    public class PropertyDescription
    {
        public bool Equals(PropertyDescription desc)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case PropertyDescription desc:
                    return this.Equals(desc);
                default:
                    return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}