#define Args
using System.Threading;
using System.Threading.Tasks;

namespace REVIEW_CSHARP_BASIC
{
    public partial class YieldExample : Form
    {
        public YieldExample()
        {
            InitializeComponent();
            Console.WriteLine(Number.GetNumber().ToArray().Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            
        }
    }
    static public class Number
    {
        static private List<int> numbers = new() { 1, 2, 3 };
        static public IEnumerable<int> GetNumber()
        {
            yield return 100;
            yield return 200;
            yield return numbers[2];
        }
    }
}