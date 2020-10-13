namespace FakerApp
{
    class Foo
    {
        private int a;
        public int b;
        private int c;
        public string Stand { get; set; }
        public char d;
        private bool e;

        public Foo(int c, string Stand, bool e)
        {
            this.c = c;
            this.Stand = Stand;
            this.e = e;
        }

        public override string ToString()
        {
            return $"private a = {a}, public b = {b}, private c = {c}, public Stand = {Stand}, public d = {d}, private e = {e}";
        }
    }
}