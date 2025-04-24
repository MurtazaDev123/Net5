using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class Account
    {
        public SmartEcommerce.Models.AccountUser ValidateUser(string UserId, string Password)
        {
            try
            {
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@UserId", Value = UserId },
                    new SqlParameter { ParameterName = "@Password", Value = SBEncryption.getMD5Password(UserId, Password) }
                };
                ErrorResponse response = new ErrorResponse();

                DataTable dt = DatabaseObject.FetchTableFromSP("ValidateLogin", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    if (DataHelper.HasRows(dt))
                    {
                        DataRow row = dt.Rows[0];
                        if (DataHelper.intParse(row["Success"]) == 1)
                        {
                            SmartEcommerce.Models.AccountUser account = new SmartEcommerce.Models.AccountUser()
                            {
                                
                                LoginId = DataHelper.intParse(row["LoginId"]),
                                UserId = UserId,
                                UserName = row["UserName"].ToString(),
                                LoginType = DataHelper.intParse(row["LoginType"])
                            };

                            return account;
                        }
                    }
                }

                return null;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }
    }
}
