using System.Linq.Expressions;

namespace PPCMD.Repositories
{
    public class QueryOption<T> where T : class
    {

        public Expression<Func<T, object>>? OrderBy { get; set; }
        public Expression<Func<T, bool>>? Where { get; set; }
        private string[] includes = Array.Empty<string>();

        public string Includes
        {
            set => includes = value.Replace(" ", "").Split(',');
        }

        public string[] GetIncludes() => includes;

        public bool HasWhere => Where != null;
        public bool HasOrderBy => OrderBy != null;
    }
}
