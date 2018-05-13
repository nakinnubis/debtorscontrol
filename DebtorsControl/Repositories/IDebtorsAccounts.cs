using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtorsControl.Repositories
{
    public interface IDebtorsAccounts
    {
        void CreateDebtor();
        void DeleteDebtor();
        void UpdateDebtor();
        void UpdateNaira();
        void UpdateDollar();
    }
}
