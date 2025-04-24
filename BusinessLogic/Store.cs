using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class Store
    {
        #region Store

        public DataTable GetAllStores(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("StoreGetAll", parem, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return dt;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Store.Store> GetStoreByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Store.Store> stores = new List<SmartEcommerce.Models.Store.Store>();
                DataTable dt = GetAllStores(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        stores.Add(new SmartEcommerce.Models.Store.Store()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Name = row["FullName"].ToString(),
                            City = new SmartEcommerce.Models.Common.City
                            {
                                Id = DataHelper.intParse(row["CityId"]),
                                Name = row["CityName"].ToString()
                            },
                            Mall = new SmartEcommerce.Models.Store.Mall
                            {
                                Id = DataHelper.intParse(row["MallId"]),
                                Name = row["MallName"].ToString()
                            },
                            Description = row["Description"].ToString(),
                            LogoImage = row["LogoImage"].ToString(),
                            Address = row["Address"].ToString(),
                            ContactNo = row["ContactNo"].ToString(),
                            Email = row["Email"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return stores;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Store.Store GetStoreById(int Id)
        {
            SmartEcommerce.Models.Store.Store stores = new SmartEcommerce.Models.Store.Store() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("StoreGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    stores.Name = row["FullName"].ToString();
                    stores.City = new SmartEcommerce.Models.Common.City
                    {
                        Id = DataHelper.intParse(row["CityId"])
                    };
                    stores.Mall = new SmartEcommerce.Models.Store.Mall
                    {
                        Id = DataHelper.intParse(row["MallId"])
                    };
                    stores.Description = row["Description"].ToString();
                    stores.LogoImage = row["LogoImage"].ToString();
                    stores.Address = row["Address"].ToString();
                    stores.ContactNo = row["ContactNo"].ToString();
                    stores.Email = row["Email"].ToString();
                    stores.Active = DataHelper.boolParse(row["IsActive"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return stores;
        }

        public bool SaveStore(int LoginId, int Id, string FullName, int CityId, int MallId, string Description, string LogoImage, string Address, string ContactNo, 
                                string Email, bool Active, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@FullName", Value = FullName },
                    new SqlParameter { ParameterName = "@CityId", Value = CityId },
                    new SqlParameter { ParameterName = "@MallId", Value = MallId },
                    new SqlParameter { ParameterName = "@Description", Value = Description },
                    new SqlParameter { ParameterName = "@LogoImage", Value = LogoImage },
                    new SqlParameter { ParameterName = "@Address", Value = Address },
                    new SqlParameter { ParameterName = "@ContactNo", Value = ContactNo },
                    new SqlParameter { ParameterName = "@Email", Value = Email },
                    new SqlParameter { ParameterName = "@IsActive", Value = Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("StoreSave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);
                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool ArchiveStore(int LoginId, int StoreId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@StoreId", Value = StoreId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("StoresMoveToArchive", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public List<SmartEcommerce.Models.Store.Store> GetMallByCity(int parent_id)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@CityId", Value = parent_id}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("GetMallByCity", parem, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                List<SmartEcommerce.Models.Store.Store> stores = new List<SmartEcommerce.Models.Store.Store>();

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        stores.Add(new SmartEcommerce.Models.Store.Store()
                        {
                            Id = DataHelper.intParse(row["Id"]),
                            Name = row["FullName"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                        });
                    }
                }

                return stores;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        #endregion Store

        #region Mall

        public DataTable GetAllMall(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] param = {

                        new SqlParameter { ParameterName = "@Status", Value = Status },


                    };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("MallGetAll", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return dt;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Store.Mall> GetMallByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Store.Mall> mall = new List<SmartEcommerce.Models.Store.Mall>();
                DataTable dt = GetAllMall(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        mall.Add(new SmartEcommerce.Models.Store.Mall()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Name = row["FullName"].ToString(),
                            City = new SmartEcommerce.Models.Common.City
                            {
                                Id = DataHelper.intParse(row["CityId"]),
                                Name = row["CityName"].ToString()
                            },
                            Description = row["Description"].ToString(),
                            MapURL = row["MapURL"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return mall;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Store.Mall GetMallById(int Id)
        {
            SmartEcommerce.Models.Store.Mall mall = new SmartEcommerce.Models.Store.Mall() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                        new SqlParameter { ParameterName = "@Id", Value = Id }
                    };

                DataTable dt = DatabaseObject.FetchTableFromSP("MallGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];
                    mall.Name = row["FullName"].ToString();
                    mall.City.Id = DataHelper.intParse(row["CityId"]);
                    mall.Description = row["Description"].ToString();
                    mall.LogoImage = row["LogoImage"].ToString();
                    mall.MapURL = row["MapURL"].ToString();
                    mall.Active = DataHelper.boolParse(row["IsActive"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return mall;
        }

        public bool SaveMall(int LoginId, SmartEcommerce.Models.Store.Mall mall, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                        new SqlParameter { ParameterName = "@Id", Value = mall.Id },

                        new SqlParameter { ParameterName = "@FullName", Value = mall.Name },
                        new SqlParameter { ParameterName = "@CityId", Value = mall.City.Id },
                        new SqlParameter { ParameterName = "@Description", Value = mall.Description },
                        new SqlParameter { ParameterName = "@LogoImage", Value = mall.LogoImage },
                        new SqlParameter { ParameterName = "@MapURL", Value = mall.MapURL },
                        new SqlParameter { ParameterName = "@IsActive", Value = mall.Active },
                        new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                    };

                DataTable dt = DatabaseObject.FetchTableFromSP("MallSave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool ArchiveMall(int LoginId, int MallId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                        new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                        new SqlParameter { ParameterName = "@MallId", Value = MallId },
                        new SqlParameter { ParameterName = "@Status", Value = Status }
                    };

                DatabaseObject.ExecuteSP("MallMoveToArchive", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }



        #endregion Mall
    }
}