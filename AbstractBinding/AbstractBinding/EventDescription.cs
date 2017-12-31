namespace AbstractBinding
{
    public class EventDescription
    {
        public bool Equals(EventDescription desc)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case EventDescription desc:
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