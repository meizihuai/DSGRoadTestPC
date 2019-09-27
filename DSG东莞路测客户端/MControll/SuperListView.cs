using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 电磁信息云服务CSharp
{
    public class SuperListView : ListView
    {
        private List<string> propertiesList;
        public SuperListView(bool autoSize = false) : base()
        {
            propertiesList = new List<string>();
            this.Dock = DockStyle.Fill;
            this.GridLines = true;
            this.FullRowSelect = true;
            this.View = View.Details;
            this.Columns.Add("序号", 50);
            if (autoSize)
            {
                this.SizeChanged += LVHelper_SizeChanged;
            }
        }
        private void LVHelper_SizeChanged(object sender, EventArgs e)
        {
            DoResize();
        }
        public void DoResize()
        {
            int _Count = this.Columns.Count - 2;
            if (this.Columns.Count >= 2)
            {
                int _Width = this.Width - this.Columns[0].Width - this.Columns[1].Width;
                for (int i = 2; i < this.Columns.Count; i++)
                {
                    ColumnHeader ch = this.Columns[i];
                    ch.Width = _Width / _Count;
                }
            }
        }

        public void BindEntity<T>()
        {

        }
        public void AddColumn<T>(string columnName, Expression<Func<T, object>> property, int width = 0)
        {
            propertiesList.Add(GetPropertyName(property));

            if (width <= 0)
            {
                this.Columns.Add(columnName);
            }
            else
            {
                this.Columns.Add(columnName, width);
            }
        }
        public static string GetPropertyName<T>(Expression<Func<T, object>> expr)
        {
            var rtn = "";
            if (expr.Body is UnaryExpression)
            {
                rtn = ((MemberExpression)((UnaryExpression)expr.Body).Operand).Member.Name;
            }
            else if (expr.Body is MemberExpression)
            {
                rtn = ((MemberExpression)expr.Body).Member.Name;
            }
            else if (expr.Body is ParameterExpression)
            {
                rtn = ((ParameterExpression)expr.Body).Type.Name;
            }
            return rtn;
        }
        public void Add<T>(T t)
        {
            if (propertiesList == null || propertiesList.Count == 0) return;
            ListViewItem itm = new ListViewItem();
            itm.Text = (this.Items.Count + 1).ToString();
            Type type = t.GetType();
            foreach (string property in propertiesList)
            {
                object obj = type.GetProperty(property).GetValue(t);
                string value = obj == null ? "" : obj.ToString();
                itm.SubItems.Add(value);
            }
            this.Items.Add(itm);
        }
        public void AddRange<T>(List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                list.ForEach(a => this.Add<T>(a));
            }
        }
    }
}
