namespace AbstractBinding
{
    internal class MethodDescription
    {
        public bool Equals(MethodDescription desc)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case MethodDescription desc:
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