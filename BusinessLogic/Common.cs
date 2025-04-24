using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace BusinessLogic
{
    public class Common
    {
        #region Country

        public DataTable GetAllCountries(int Status = BusinessLogic.Status.Current)
        {
            //this is git commit and gitignore file for testing 123 456 789 10
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("CountryGetAll", parem, ref response);

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

        public List<SmartEcommerce.Models.Common.Country> GetCountryByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.Country> countries = new List<SmartEcommerce.Models.Common.Country>();
                DataTable dt = GetAllCountries(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        countries.Add(new SmartEcommerce.Models.Common.Country()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Code = row["Code"].ToString(),
                            Title = row["FullName"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"]),
                            Masking = row["Masking"].ToString(),
                            Currency = row["Currency"].ToString()
                        });
                    }
                }

                return countries;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Common.Country> GetCountryBySession(string ip_address, bool is_mobile_api = false)
        {
            try
            {
                ip_address = ip_address.Replace("\n", "");
                List<SmartEcommerce.Models.Common.Country> countries = new List<SmartEcommerce.Models.Common.Country>();

                DataTable dt = DataHelper.GetCountryFromSession(ip_address, is_mobile_api);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        countries.Add(new SmartEcommerce.Models.Common.Country()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Code = row["Code"].ToString(),
                            Title = row["FullName"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"]),
                            Masking = row["Masking"].ToString(),
                            Currency = row["Currency"].ToString()
                        });
                    }
                }

                return countries;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Common.Country GetCountryById(int Id)
        {
            SmartEcommerce.Models.Common.Country countries = new SmartEcommerce.Models.Common.Country() {Id = Id};

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("CountryGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];
                    
                    countries.Code = row["Code"].ToString();
                    countries.Name = row["FullName"].ToString();
                    countries.Active = DataHelper.boolParse(row["IsActive"]);
                    countries.Masking = row["Masking"].ToString();
                    countries.Currency = row["Currency"].ToString();
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return countries;
        }

        public bool SaveCountry(int LoginId, int Id, string Code, string FullName, string Masking, string Currency, bool Active, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Code", Value = Code },
                    new SqlParameter { ParameterName = "@FullName", Value = FullName },
                    new SqlParameter { ParameterName = "@Masking", Value = Masking },
                    new SqlParameter { ParameterName = "@Currency", Value = Currency },
                    new SqlParameter { ParameterName = "@IsActive", Value = Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("CountrySave", param, ref response);
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

        public bool ArchiveCountry(int LoginId, int CountryId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("CountriesMoveToArchive", param, ref response);
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

        public SmartEcommerce.Models.Common.Country GetCountryCurrency(int Id)
        {
            SmartEcommerce.Models.Common.Country countries = new SmartEcommerce.Models.Common.Country() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();

                DataTable dt = DatabaseObject.FetchTable("SELECT Currency FROM Countries WHERE Id = '" + Id + "'", ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    countries.Currency = row["Currency"].ToString();
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return countries;
        }

        #endregion Country

        #region State

        public DataTable GetAllStates(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("StateGetAll", parem, ref response);

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

        public List<SmartEcommerce.Models.Common.State> GetStateByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.State> states = new List<SmartEcommerce.Models.Common.State>();
                DataTable dt = GetAllStates(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        states.Add(new SmartEcommerce.Models.Common.State()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Code = row["Code"].ToString(),
                            Name = row["FullName"].ToString(),
                            Country = new SmartEcommerce.Models.Common.Country
                            {
                                Id = DataHelper.intParse(row["CountryId"]),
                                Name = row["CountryName"].ToString()
                            },
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return states;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Common.State> GetStateByCountryId(int CountryId)
        {
            try
            {
                List<SmartEcommerce.Models.Common.State> states = new List<SmartEcommerce.Models.Common.State>();
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("GetStatesByCountryId", parem, ref response);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        states.Add(new SmartEcommerce.Models.Common.State()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Code = row["Code"].ToString(),
                            Name = row["FullName"].ToString(),
                            Country = new SmartEcommerce.Models.Common.Country
                            {
                                Id = DataHelper.intParse(row["CountryId"]),
                                Name = row["CountryName"].ToString()
                            },
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return states;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Common.State GetStateById(int Id)
        {
            SmartEcommerce.Models.Common.State states = new SmartEcommerce.Models.Common.State() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("StateGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    states.Code = row["Code"].ToString();
                    states.Name = row["FullName"].ToString();
                    states.Country = new SmartEcommerce.Models.Common.Country
                    {
                        Id = DataHelper.intParse(row["CountryId"])
                    };
                    states.Active = DataHelper.boolParse(row["IsActive"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return states;
        }

        public bool SaveState(int LoginId, int Id, string Code, string FullName, int CountryId, bool Active, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Code", Value = Code },
                    new SqlParameter { ParameterName = "@FullName", Value = FullName },
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId },
                    new SqlParameter { ParameterName = "@IsActive", Value = Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("StateSave", param, ref response);
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

        public bool ArchiveState(int LoginId, int StateId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@StateId", Value = StateId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("StatesMoveToArchive", param, ref response);
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

        public List<SmartEcommerce.Models.Common.State> GetStateByCountry(int parent_id)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@CountryId", Value = parent_id}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("GetStatesByCountry", parem, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                List<SmartEcommerce.Models.Common.State> states = new List<SmartEcommerce.Models.Common.State>();

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        states.Add(new SmartEcommerce.Models.Common.State()
                        {
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["FullName"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                        });
                    }
                }

                return states;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        #endregion State

        #region City

        public DataTable GetAllCities(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("CityGetAll", parem, ref response);

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

        public List<SmartEcommerce.Models.Common.City> GetCityByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.City> cities = new List<SmartEcommerce.Models.Common.City>();
                DataTable dt = GetAllCities(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cities.Add(new SmartEcommerce.Models.Common.City()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Code = row["Code"].ToString(),
                            Name = row["FullName"].ToString(),
                            Country = new SmartEcommerce.Models.Common.Country
                            {
                                Id = DataHelper.intParse(row["CountryId"]),
                                Name = row["CountryName"].ToString()
                            },
                            State = new SmartEcommerce.Models.Common.State
                            {
                                Id = DataHelper.intParse(row["StateId"]),
                                Name = row["StateName"].ToString()
                            },
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return cities;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Common.City> GetCityByCountryStateId(int CountryId, int StateId)
        {
            try
            {
                List<SmartEcommerce.Models.Common.City> cities = new List<SmartEcommerce.Models.Common.City>();

                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId},
                    new SqlParameter { ParameterName = "@StateId", Value = StateId}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("GetCityByCountryStateId", parem, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cities.Add(new SmartEcommerce.Models.Common.City()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Code = row["Code"].ToString(),
                            Name = row["FullName"].ToString(),
                            Country = new SmartEcommerce.Models.Common.Country
                            {
                                Id = DataHelper.intParse(row["CountryId"]),
                                Name = row["CountryName"].ToString()
                            },
                            State = new SmartEcommerce.Models.Common.State
                            {
                                Id = DataHelper.intParse(row["StateId"]),
                                Name = row["StateName"].ToString()
                            },
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return cities;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Common.City GetCityById(int Id)
        {
            SmartEcommerce.Models.Common.City cities = new SmartEcommerce.Models.Common.City() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("CityGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    cities.Code = row["Code"].ToString();
                    cities.Name = row["FullName"].ToString();
                    cities.Country = new SmartEcommerce.Models.Common.Country
                    {
                        Id = DataHelper.intParse(row["CountryId"])
                    };
                    cities.State = new SmartEcommerce.Models.Common.State
                    {
                        Id = DataHelper.intParse(row["StateId"])
                    };
                    cities.Active = DataHelper.boolParse(row["IsActive"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return cities;
        }

        public bool SaveCity(int LoginId, int Id, string Code, string FullName, int CountryId, int StateId, bool Active, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Code", Value = Code },
                    new SqlParameter { ParameterName = "@FullName", Value = FullName },
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId },
                    new SqlParameter { ParameterName = "@StateId", Value = StateId },
                    new SqlParameter { ParameterName = "@IsActive", Value = Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("CitySave", param, ref response);
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

        public bool ArchiveCity(int LoginId, int CityId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@CityId", Value = CityId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("CitiesMoveToArchive", param, ref response);
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

        public List<SmartEcommerce.Models.Common.City> GetCitiesByState(int parent_id)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@StateId", Value = parent_id}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("GetCityByState", parem, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                List<SmartEcommerce.Models.Common.City> cities = new List<SmartEcommerce.Models.Common.City>();

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cities.Add(new SmartEcommerce.Models.Common.City()
                        {
                            Id = DataHelper.intParse(row["Id"]),
                            Name = row["FullName"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                        });
                    }
                }

                return cities;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        #endregion City

        #region Sliders

        public DataTable GetAllSliders(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("SliderGetAll", parem, ref response);

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

        public List<SmartEcommerce.Models.Common.Sliders> GetSliderByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.Sliders> sliders = new List<SmartEcommerce.Models.Common.Sliders>();
                DataTable dt = GetAllSliders(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        sliders.Add(new SmartEcommerce.Models.Common.Sliders()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            PageName = row["PageName"].ToString(),
                            ImageURL = row["ImageURL"].ToString(),
                            Sequence = DataHelper.intParse(row["Sequence"]),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return sliders;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Common.Sliders GetSliderById(int Id)
        {
            SmartEcommerce.Models.Common.Sliders sliders = new SmartEcommerce.Models.Common.Sliders() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("SliderGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    sliders.Title = row["Title"].ToString();
                    sliders.PageName = row["PageName"].ToString();
                    sliders.ImageURL = row["ImageURL"].ToString();
                    sliders.RedirectionURL = row["RedirectionURL"].ToString();
                    sliders.Sequence = DataHelper.intParse(row["Sequence"].ToString());
                    sliders.Active = DataHelper.boolParse(row["IsActive"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return sliders;
        }

        public bool SaveSlider(int LoginId, int Id, string Title, string PageName, string ImageURL, string RedirectionURL, int Sequence, bool Active, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Title", Value = Title },
                    new SqlParameter { ParameterName = "@PageName", Value = PageName },
                    new SqlParameter { ParameterName = "@ImageURL", Value = ImageURL },
                    new SqlParameter { ParameterName = "@RedirectionURL", Value = RedirectionURL },
                    new SqlParameter { ParameterName = "@Sequence", Value = Sequence },
                    new SqlParameter { ParameterName = "@IsActive", Value = Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("SliderSave", param, ref response);
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

        public bool ArchiveSlider(int LoginId, int SliderId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@SliderId", Value = SliderId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("SlidersMoveToArchive", param, ref response);
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

        #endregion Sliders

        #region Topics

        public DataTable GetAllTopics(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("TopicGetAll", parem, ref response);

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

        public List<SmartEcommerce.Models.Common.Topic> GetTopicByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.Topic> topics = new List<SmartEcommerce.Models.Common.Topic>();
                DataTable dt = GetAllTopics(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        topics.Add(new SmartEcommerce.Models.Common.Topic()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return topics;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Common.Topic GetTopicById(int Id)
        {
            SmartEcommerce.Models.Common.Topic topics = new SmartEcommerce.Models.Common.Topic() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("TopicGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    topics.Title = row["Title"].ToString();
                    topics.Active = DataHelper.boolParse(row["IsActive"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return topics;
        }

        public bool SaveTopic(int LoginId, int Id, string Title, bool Active, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Title", Value = Title },
                    new SqlParameter { ParameterName = "@IsActive", Value = Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("TopicSave", param, ref response);
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

        public bool ArchiveTopic(int LoginId, int TopicId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@TopicId", Value = TopicId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("TopicsMoveToArchive", param, ref response);
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

        #endregion Topics

        #region Users

        public DataTable GetAllUsers(int Status = BusinessLogic.Status.Current, int LoginType = BusinessLogic.LoginType.User)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status},
                    new SqlParameter { ParameterName = "@LoginType", Value = LoginType}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("UserGetAll", parem, ref response);

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

        public List<SmartEcommerce.Models.Common.User> GetUserByStatus(int Status, int LoginType)
        {
            try
            {
                List<SmartEcommerce.Models.Common.User> topics = new List<SmartEcommerce.Models.Common.User>();
                DataTable dt = GetAllUsers(Status, LoginType);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        topics.Add(new SmartEcommerce.Models.Common.User()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            UserId = row["UserId"].ToString(),
                            UserName = row["UserName"].ToString(),
                            Title = row["UserName"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["CreatedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["CreatedOn"]),
                            LastLogin = DataHelper.dateParse(row["LastLogin"])
                        });
                    }
                }

                return topics;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Common.User GetUserById(int Id)
        {
            SmartEcommerce.Models.Common.User users = new SmartEcommerce.Models.Common.User() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("UserGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    users.UserId = row["UserId"].ToString();
                    users.UserName = row["UserName"].ToString();
                    users.PhoneNo = row["PhoneNo"].ToString();
                    users.DateOfBirth = DataHelper.dateParse(row["DateOfBirth"]).ToString("yyyy-MM-dd");
                    users.Gender = row["Gender"].ToString();
                    users.Address = row["Address"].ToString();
                    users.Active = DataHelper.boolParse(row["IsActive"]);
                    users.Approval = DataHelper.boolParse(row["IsApproval"]);
                    users.ProfilePicture = row["ProfilePicture"].ToString();
                    users.CountryName = row["CountryName"].ToString();
                    users.CountryId = DataHelper.intParse(row["CountryId"]);
                    users.StateId = DataHelper.intParse(row["StateId"]);
                    users.CityId = DataHelper.intParse(row["CityId"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return users;
        }

        public bool SaveUser(int LoginId, SmartEcommerce.Models.Common.Partner partner, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                string EncPass = SBEncryption.getMD5Password(partner.EmailAddress, partner.Password);
                string ProfilePicture = "";

                if (partner.ProfilePicture == "")
                    ProfilePicture = DataHelper.GenerateProfilePicture(partner.FullName);
                else
                    ProfilePicture = partner.ProfilePicture;



                #region Categories
                DataTable dTableCategories = new DataTable();
                dTableCategories.Columns.Add("Id", typeof(int));
                dTableCategories.Columns.Add("CategoryId", typeof(string));

                foreach (SmartEcommerce.Models.Product.Category pcTrans in partner.PartnerContentTypeUpload)
                {
                    DataRow new_row = dTableCategories.NewRow();

                    new_row["Id"] = pcTrans.Id;
                    new_row["CategoryId"] = pcTrans.ContentTypeUploadId;

                    dTableCategories.Rows.Add(new_row);
                }
                #endregion

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = partner.Id },
                    new SqlParameter { ParameterName = "@UserName", Value = partner.FullName },
                    new SqlParameter { ParameterName = "@PartnerType", Value = partner.PartnerType.Id },
                    new SqlParameter { ParameterName = "@PartnerCategory", Value = partner.PartnerCategory.Id },
                    new SqlParameter { ParameterName = "@ContactPerson", Value = partner.ContactPerson },
                    new SqlParameter { ParameterName = "@Telephone", Value = partner.Telephone },
                    new SqlParameter { ParameterName = "@MobileNo", Value = partner.MobileNo },
                    new SqlParameter { ParameterName = "@UserId", Value = partner.EmailAddress },
                    new SqlParameter { ParameterName = "@Password", Value = EncPass },
                    new SqlParameter { ParameterName = "@Country", Value = partner.Country.Id },
                    new SqlParameter { ParameterName = "@State", Value = partner.State.Id },
                    new SqlParameter { ParameterName = "@City", Value = partner.City.Id },
                    new SqlParameter { ParameterName = "@Address", Value = partner.Address },
                    new SqlParameter { ParameterName = "@PartnerContentType", Value = partner.PartnerContentType.Id },
                    new SqlParameter { ParameterName = "@IsActive", Value = partner.Active },
                    new SqlParameter { ParameterName = "@IsApproval", Value = partner.Approval },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@IsPassChange", Value = partner.IsPassChange },
                    new SqlParameter { ParameterName = "@ProfilePicture", Value = ProfilePicture },
                    new SqlParameter { ParameterName = "@Monetization", Value = partner.Monetization },
                    new SqlParameter { ParameterName = "@ContentTypeUpload", Value = dTableCategories, SqlDbType = SqlDbType.Structured, TypeName = "type_ContentTypeUpload" }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("UserSave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);

                if (entryLevel == 1)
                {
                    string admin_email = ConfigurationManager.AppSettings["admin_email"];

                    StringBuilder sbHtml = new StringBuilder();
                    var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/partner_request.html");
                    sbHtml.AppendLine(System.IO.File.ReadAllText(path));
                    sbHtml = sbHtml.Replace("{NAME}", partner.FullName);
                    Emails.SendMail(partner.EmailAddress, admin_email, "NetFive - Want's to Become a Partner!", sbHtml.ToString(), true);
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool ArchiveUser(int LoginId, int UserId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@UserId", Value = UserId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("UsersMoveToArchive", param, ref response);
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

        #endregion Users

        #region Disputed Videos

        public List<SmartEcommerce.Models.Common.DisputeVideo> GetReportVideos()
        {
            try
            {
                List<SmartEcommerce.Models.Common.DisputeVideo> videos = new List<SmartEcommerce.Models.Common.DisputeVideo>();

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("ReportVideoGetAll", null, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        videos.Add(new SmartEcommerce.Models.Common.DisputeVideo()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            VideoId = DataHelper.intParse(row["VideoId"].ToString()),
                            VideoTitle = row["VideoTitle"].ToString(),
                            VideoURL = row["VideoURL"].ToString(),
                            CustomerId = DataHelper.intParse(row["CustomerId"].ToString()),
                            CustomerName = row["CustomerName"].ToString(),
                            Description = row["Description"].ToString(),
                            TopicName = row["TopicName"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"])
                        });
                    }
                }

                return videos;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Common.DisputeVideo> GetClaimVideos(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.DisputeVideo> videos = new List<SmartEcommerce.Models.Common.DisputeVideo>();
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("ClaimVideoGetAll", parem, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        videos.Add(new SmartEcommerce.Models.Common.DisputeVideo()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            VideoId = DataHelper.intParse(row["VideoId"].ToString()),
                            VideoTitle = row["VideoTitle"].ToString(),
                            VideoURL = row["VideoURL"].ToString(),
                            CustomerId = DataHelper.intParse(row["CustomerId"].ToString()),
                            CustomerName = row["CustomerName"].ToString(),
                            Description = row["Description"].ToString(),
                            TopicName = row["TopicName"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            CustomerEmail = row["CustomerEmail"].ToString()
                        });
                    }
                }

                return videos;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Common.DisputeVideo GetClaimDescription(int Id)
        {
            SmartEcommerce.Models.Common.DisputeVideo claims = new SmartEcommerce.Models.Common.DisputeVideo() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();

                DataTable dt = DatabaseObject.FetchTable("SELECT Description FROM VideosDispute WHERE Id = '" + Id + "'", ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    claims.Description = row["Description"].ToString();
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return claims;
        }

        #endregion

        #region Customers

        public DataTable GetAllCustomers()
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("CustomerGetAll", null, ref response);

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

        public List<SmartEcommerce.Models.Common.User> GetCustomer()
        {
            try
            {
                List<SmartEcommerce.Models.Common.User> customers = new List<SmartEcommerce.Models.Common.User>();
                DataTable dt = GetAllCustomers();

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        customers.Add(new SmartEcommerce.Models.Common.User()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            UserId = row["UserId"].ToString(),
                            UserName = row["UserName"].ToString(),
                            PhoneNo = row["PhoneNo"].ToString(),
                            DateOfBirth = (string.IsNullOrEmpty(row["DateOfBirth"].ToString()) ? DataHelper.dateParse(row["DateOfBirth"]).ToString("yyyy-MM-dd") : ""),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedOn = DataHelper.dateParse(row["CreatedOn"]),
                            CountryName = row["CountryName"].ToString(),
                            StateName = row["StateName"].ToString(),
                            CityName = row["CityName"].ToString(),
                            Gender = row["Gender"].ToString(),
                            SubscriptionType = row["SubscriptionType"].ToString(),
                            SubscriptionStartDate = (String.IsNullOrEmpty(row["SubscriptionStart"].ToString()) ? "" : DataHelper.dateParse(row["SubscriptionStart"]).ToString("dd-MMM-yyyy")),
                            SubscriptionEndDate = (String.IsNullOrEmpty(row["SubscriptionEnd"].ToString()) ? "" : DataHelper.dateParse(row["SubscriptionEnd"]).ToString("dd-MMM-yyyy")),
                            SubscriptionStatus = row["SubscriptionStatus"].ToString()
                        });
                    }
                }

                return customers;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public int GetCustomerCountry()
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId } };
                int result = DataHelper.intParse(DatabaseObject.DLookupDB("Web_GetCountryIdByCustomer", param, ref response));

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return 0;
                }

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return 0;
            }
        }

        #endregion

        #region Subscription Setup

        public DataTable GetAllSubscription(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("SubscriptionGetAll", parem, ref response);

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

        public List<SmartEcommerce.Models.Common.Subscription> GetSubscriptionByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.Subscription> subscriptions = new List<SmartEcommerce.Models.Common.Subscription>();
                DataTable dt = GetAllSubscription(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        subscriptions.Add(new SmartEcommerce.Models.Common.Subscription()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            YearlyRate = DataHelper.decimalParse(row["YearlyRate"]),
                            MonthlyRate = DataHelper.decimalParse(row["MonthlyRate"]),
                            Country = new SmartEcommerce.Models.Common.Country
                            {
                                Id = DataHelper.intParse(row["CountryId"]),
                                Name = row["CountryName"].ToString(),
                                Currency = row["Currency"].ToString()
                            },
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return subscriptions;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public bool ArchiveSubscription(int LoginId, int SubscriptionId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@SubscriptionId", Value = SubscriptionId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("SubscriptionMoveToArchive", param, ref response);
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

        public SmartEcommerce.Models.Common.Subscription GetSubscriptionById(int Id)
        {
            SmartEcommerce.Models.Common.Subscription subscription = new SmartEcommerce.Models.Common.Subscription() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("SubscriptionGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];
                    subscription.SubType = row["SubType"].ToString();
                    subscription.YearlyRate = DataHelper.decimalParse(row["YearlyRate"]);
                    subscription.MonthlyRate = DataHelper.decimalParse(row["MonthlyRate"]);
                    subscription.Country = new SmartEcommerce.Models.Common.Country
                    {
                        Id = DataHelper.intParse(row["CountryId"]),
                        Currency = row["Currency"].ToString()
                    };
                    subscription.Active = DataHelper.boolParse(row["IsActive"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return subscription;
        }

        public string SaveSubscription(int LoginId, int Id, string SubType, int CountryId, string Currency, decimal YearlyRate, decimal MonthlyRate, bool Active, out int entryLevel)
        {
            entryLevel = 0;

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@SubType", Value = SubType },
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId },
                    new SqlParameter { ParameterName = "@Currency", Value = Currency },
                    new SqlParameter { ParameterName = "@YearlyRate", Value = YearlyRate },
                    new SqlParameter { ParameterName = "@MonthlyRate", Value = MonthlyRate },
                    new SqlParameter { ParameterName = "@IsActive", Value = Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("SubscriptionSave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return "999";
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);
                return dt.Rows[0]["ErrorCode"].ToString();
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "999";
            }
        }

        public SmartEcommerce.Models.Common.Subscription GetGlobalRates()
        {
            SmartEcommerce.Models.Common.Subscription subscription = new SmartEcommerce.Models.Common.Subscription();

            try
            {
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("GetGlobalRates", null, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    subscription.MonthlyRate = DataHelper.intParse(row["MonthlyRate"]);
                    subscription.YearlyRate = DataHelper.intParse(row["YearlyRate"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return subscription;
        }

        public SmartEcommerce.Models.Common.Subscription GetSubscriptionPlan(int CountryId)
        {

            SmartEcommerce.Models.Common.Subscription subscription = new SmartEcommerce.Models.Common.Subscription();

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId },
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("GetSubscriptionPlanByCountryId", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                DataTable dt = dtbls[0];

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    subscription.MonthlyRate = DataHelper.intParse(row["MonthlyRate"]);
                    subscription.YearlyRate = DataHelper.intParse(row["YearlyRate"]);
                    subscription.Country = new SmartEcommerce.Models.Common.Country
                    {
                        Currency = row["Currency"].ToString()
                    };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return subscription;
        }

        public SmartEcommerce.Models.Common.Subscription GetSubscriptionStatus(int LoginId)
        {
            SmartEcommerce.Models.Common.Subscription subscription = new SmartEcommerce.Models.Common.Subscription();

            try
            {
                ErrorResponse response = new ErrorResponse();

                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("GetSubscriptionStatus", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    subscription.SubStatus = row["SubscriptionStatus"].ToString();
                    subscription.SubType = row["SubscriptionType"].ToString();
                    subscription.Country = new SmartEcommerce.Models.Common.Country
                    {
                        Id = DataHelper.intParse(row["CountryId"]),
                    };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return subscription;
        }

        public SmartEcommerce.Models.Common.Subscription GetSubscriptionPlanCustomer()
        {

            SmartEcommerce.Models.Common.Subscription subscription = new SmartEcommerce.Models.Common.Subscription();

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId },
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("GetSubscriptionPlanByCustomer", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                DataTable dt = dtbls[0];

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    subscription.SubType = row["SubscriptionType"].ToString();
                    subscription.SubPlan = row["SubscriptionPlan"].ToString();
                    subscription.SubAmount = DataHelper.intParse(row["SubscriptionAmount"]);
                    subscription.SubStartDate = DataHelper.dateParse(row["SubscriptionStart"]).ToString("dd-MMM-yyyy");
                    subscription.SubEndDate = DataHelper.dateParse(row["SubscriptionEnd"]).ToString("dd-MMM-yyyy");
                    subscription.SubStatus = row["SubscriptionStatus"].ToString();
                    subscription.Country = new SmartEcommerce.Models.Common.Country
                    {
                        Id = DataHelper.intParse(row["CountryId"]),
                        Name = row["CountryName"].ToString(),
                        Currency = row["Currency"].ToString()
                    };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return subscription;
        }

        public string ActiveSubscriptionPlan(int CustomerId, string SubscriptionPlan, decimal SubscriptionPrice)
        {

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@CustomerId", Value = CustomerId },
                    new SqlParameter { ParameterName = "@SubscriptionPlan", Value = SubscriptionPlan },
                    new SqlParameter { ParameterName = "@SubscriptionPrice", Value = SubscriptionPrice },
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("SubscriptionPlanActive", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return "999";
                }

                return dt.Rows[0]["ErrorCode"].ToString();
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "999";
            }
        }

        public string CancelSubscriptionPlan(int CustomerId)
        {

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@CustomerId", Value = CustomerId },
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("SubscriptionPlanCancel", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return "999";
                }

                return dt.Rows[0]["ErrorCode"].ToString();
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "999";
            }
        }

        public string ReActiveSubscriptionPlan(int CustomerId)
        {

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@CustomerId", Value = CustomerId },
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("SubscriptionPlanReActive", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return "999";
                }

                return dt.Rows[0]["ErrorCode"].ToString();
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "999";
            }
        }

        public string PaymentSubscription(string customer_id, decimal stripe_amount, string client_secret, string stripe_currency, string payment_intent_id, 
            string payment_method, string payment_status, string error_code, string error_message, string decline_code, string charge)
        {

            try
            {

                stripe_amount = (stripe_amount / 100); // for stripe functionality
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@StripeCustomerId", Value = customer_id },
                    new SqlParameter { ParameterName = "@StripeAmount", Value = stripe_amount },
                    new SqlParameter { ParameterName = "@ClientSecret", Value = client_secret },
                    new SqlParameter { ParameterName = "@StripeCurrency", Value = stripe_currency },
                    new SqlParameter { ParameterName = "@PaymentIntentId", Value = payment_intent_id },
                    new SqlParameter { ParameterName = "@PaymentMethod", Value = payment_method },
                    new SqlParameter { ParameterName = "@PaymentStatus", Value = payment_status },
                    new SqlParameter { ParameterName = "@ErrorCode", Value = error_code },
                    new SqlParameter { ParameterName = "@ErrorMessage", Value = error_message },
                    new SqlParameter { ParameterName = "@DeclineCode", Value = decline_code },
                    new SqlParameter { ParameterName = "@Charge", Value = charge },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId },
                    new SqlParameter { ParameterName = "@SubscriptionPlan", Value = SBSession.GetSessionValue("SessionPlanType") },
                    new SqlParameter { ParameterName = "@StartDate", Value = DBNull.Value }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("PaymentSubscription", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return "999";
                }
                else
                {
                    string ErrorCode = dt.Rows[0]["ErrorCode"].ToString();

                    if (ErrorCode == "000")
                    {
                        long PaymentId = DataHelper.longParse(dt.Rows[0]["PaymentId"]);
                        string receipt_no = PaymentId.ToString("000000");

                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/payment-success.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                        sbHtml = sbHtml.Replace("{NAME}", clsWebSession.UserName);
                        sbHtml = sbHtml.Replace("{CURRENCY}", stripe_currency.ToUpper());
                        sbHtml = sbHtml.Replace("{AMOUNT}", stripe_amount.ToString("#,##0.00"));
                        sbHtml = sbHtml.Replace("{RECEIPT_NO}", receipt_no);

                        bool IsSend = Emails.SendMail(clsWebSession.UserId, "NetFive - Payment Receipt!", sbHtml.ToString(), true);
                    }
                    else
                    {
                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/payment-failed.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));
                        
                        bool IsSend = Emails.SendMail(clsWebSession.UserId, "NetFive - Payment failed!", sbHtml.ToString(), true);
                    }

                    return dt.Rows[0]["ErrorCode"].ToString();
                }

                
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "999";
            }
        }

        #endregion

        #region Sysytem Configuration

        public SmartEcommerce.Models.Common.SystemConfiguration GetParameterValue(string ParameterName)
        {
            SmartEcommerce.Models.Common.SystemConfiguration configuration = new SmartEcommerce.Models.Common.SystemConfiguration() { ParameterName = ParameterName };

            try
            {
                ErrorResponse response = new ErrorResponse();

                DataTable dt = DatabaseObject.FetchTable("SELECT ParameterValue FROM SystemConfiguration WHERE ParameterName = '" + ParameterName + "'", ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    configuration.ParameterValue = row["ParameterValue"].ToString();
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return configuration;
        }

        public bool SaveParameterValue(string ParameterName, string ParameterValue)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@ParameterName", Value = ParameterName },
                    new SqlParameter { ParameterName = "@ParameterValue", Value = ParameterValue },
                };

                DatabaseObject.ExecuteSP("SystemConfigurationUpdate", param, ref response);
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

        public SmartEcommerce.Models.Common.Settings GetSettings()
        {
            SmartEcommerce.Models.Common.Settings settings = new SmartEcommerce.Models.Common.Settings() { };

            try
            {
                ErrorResponse response = new ErrorResponse();

                DataTable dt = DatabaseObject.FetchTable("GetSettings", ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    settings.MonthlyRate = row["MonthlyRate"].ToString();
                    settings.YearlyRate = row["YearlyRate"].ToString();
                    settings.IsTrial = row["SubscriptionTrial"].ToString();
                    settings.TrialDays = row["SubscriptionTrialDays"].ToString();
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return settings;
        }

        public bool SaveSettings(string MonthlyRate, string YearlyRate, string IsTrial, string TrialDays)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@MonthlyRate", Value = MonthlyRate },
                    new SqlParameter { ParameterName = "@YearlyRate", Value = YearlyRate },
                    new SqlParameter { ParameterName = "@IsTrial", Value = IsTrial },
                    new SqlParameter { ParameterName = "@TrialDays", Value = TrialDays }
                };

                DatabaseObject.ExecuteSP("SettingSave", param, ref response);
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

        #endregion

        #region MyAccount

        public bool UpdateProfile(int LoginId, SmartEcommerce.Models.Common.User user)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@UserName", Value = user.UserName },
                    new SqlParameter { ParameterName = "@DateOfBirth", Value = user.DateOfBirth },
                    new SqlParameter { ParameterName = "@Gender", Value = user.Gender },
                    new SqlParameter { ParameterName = "@StateId", Value = user.StateId },
                    new SqlParameter { ParameterName = "@CityId", Value = user.CityId },
                    new SqlParameter { ParameterName = "@Address", Value = user.Address },
                    new SqlParameter { ParameterName = "@ProfilePicture", Value = user.ProfilePicture },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("UpdateProfile", param, ref response);
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

        #endregion

        #region Channels Subscriber

        public dynamic SubscriberRequest(int SubscriberId, int LoginId, int Subscribe)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@SubscriberId", Value = SubscriberId },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Subscribe", Value = Subscribe }
                };

                DataTable dataTable = DatabaseObject.FetchTableFromSP("SubscribeRequest", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    return DataHelper.FormatNumber(DataHelper.longParse(dataTable.Rows[0]["TotalSubscribers"]));
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public bool SubscriberNotification(int SubscriberId, int LoginId, bool Notification)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = 
                {
                    new SqlParameter { ParameterName = "@SubscriberId", Value = SubscriberId },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Notification", Value = Notification }
                };

                if (DatabaseObject.ExecuteSP_("SubscriberNotification", param, ref response))
                {
                    return true;
                }
                else
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public dynamic LikeVideo(long VideoId, int LoginId, int Type, bool IsAdd, bool IsLiveStreaming)
        {
            try
            {
                ErrorResponse response = new BusinessLogic.ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId },
                    new SqlParameter { ParameterName = "@Loginid", Value = LoginId },
                    new SqlParameter { ParameterName = "@Type", Value = Type },
                    new SqlParameter { ParameterName = "@IsAdd", Value = IsAdd }
                };

                string storedProcedure = "LikeVideo";
                if (IsLiveStreaming)
                    storedProcedure = "LikeVideo_LiveStreaming";

                DataTable dataTable = DatabaseObject.FetchTableFromSP(storedProcedure, param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    long totalLikes = DataHelper.longParse(dataTable.Rows[0]["TotalLikes"]);
                    long totalDislikes = DataHelper.longParse(dataTable.Rows[0]["TotalDislikes"]);
                    var obj = new
                    {
                        Likes = DataHelper.FormatNumber(totalLikes),
                        Dislikes = DataHelper.FormatNumber(totalDislikes),
                        IsLike = DataHelper.boolParse(dataTable.Rows[0]["IsLike"]),
                        IsDisLike = DataHelper.boolParse(dataTable.Rows[0]["IsDisLike"]),
                        Message = dataTable.Rows[0]["Message"].ToString()
                    };

                    return obj;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        #endregion

        #region Partner

        public List<SmartEcommerce.Models.Common.Partner> GetPartnersByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.Partner> partners = new List<SmartEcommerce.Models.Common.Partner>();

                SqlParameter[] param = {
                    new SqlParameter { ParameterName = "@Status", Value = Status },
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("PartnersGetAll", param, ref response);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        partners.Add(new SmartEcommerce.Models.Common.Partner()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            FullName = row["UserName"].ToString(),
                            PartnerType = new SmartEcommerce.Models.Common.PartnerType
                            {
                                Id = DataHelper.intParse(row["PartnerTypeId"]),
                                Title = row["PartnerTypeName"].ToString()
                            },
                            PartnerCategory = new SmartEcommerce.Models.Common.PartnerCategory
                            {
                                Id = DataHelper.intParse(row["PartnerCateoryId"]),
                                Title = row["PartnerCateoryName"].ToString()
                            },
                            Country = new SmartEcommerce.Models.Common.Country
                            {
                                Id = DataHelper.intParse(row["CountryId"]),
                                Title = row["CountryName"].ToString()
                            },
                            EmailAddress = row["Email"].ToString(),
                            MobileNo = row["PhoneNo"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["CreatedOn"]),
                            Active = DataHelper.boolParse(row["IsActive"])
                        });
                    }
                }

                return partners;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Common.Partner GetPartnerById(int Id)
        {
            SmartEcommerce.Models.Common.Partner partners = new SmartEcommerce.Models.Common.Partner() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("PartnerGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dtbls[0]))
                {
                    DataRow row = dtbls[0].Rows[0];

                    partners.Id = DataHelper.intParse(row["Id"]);
                    partners.FullName = row["UserName"].ToString();
                    partners.PartnerType = new SmartEcommerce.Models.Common.PartnerType
                    {
                        Id = DataHelper.intParse(row["PartnerTypeId"]),
                        Title = row["PartnerTypeName"].ToString()
                    };

                    partners.PartnerCategory = new SmartEcommerce.Models.Common.PartnerCategory
                    {
                        Id = DataHelper.intParse(row["PartnerCateoryId"]),
                        Title = row["PartnerCateoryName"].ToString()
                    };

                    partners.ContactPerson = row["ContactPerson"].ToString();
                    partners.Telephone = row["Telephone"].ToString();
                    partners.MobileNo = row["PhoneNo"].ToString();
                    partners.EmailAddress = row["Email"].ToString();

                    partners.Country = new SmartEcommerce.Models.Common.Country
                    {
                        Id = DataHelper.intParse(row["CountryId"]),
                        Title = row["CountryName"].ToString()
                    };
                    partners.State = new SmartEcommerce.Models.Common.State
                    {
                        Id = DataHelper.intParse(row["StateId"]),
                        Title = row["StateName"].ToString()
                    };
                    partners.City = new SmartEcommerce.Models.Common.City
                    {
                        Id = DataHelper.intParse(row["CityId"]),
                        Name = row["CityName"].ToString()
                    };

                    partners.Address = row["Address"].ToString();

                    partners.PartnerContentType = new SmartEcommerce.Models.Common.PartnerContentType
                    {
                        Id = DataHelper.intParse(row["ContentTypeId"]),
                        Title = row["ContentTypeName"].ToString()
                    };


                    foreach (DataRow row2 in dtbls[1].Rows)
                    {
                        partners.PartnerContentTypeUpload.Add(new SmartEcommerce.Models.Product.Category()
                        {
                            Id = DataHelper.intParse(row2["Id"]),
                            ContentTypeUploadId = DataHelper.stringParse(row2["CategoryId"]),
                        });
                    }

                    //partners.PartnerContentTypeUpload = new SmartEcommerce.Models.Product.Category
                    //{
                    //    Id = DataHelper.intParse(row["ContentTypeUploadId"]),
                    //    Title = row["ContentTypeUploadName"].ToString()
                    //};

                    partners.PartnerContentTypeUploadName = row["ContentTypeUploadName"].ToString();

                    partners.ProfilePicture = row["ProfilePicture"].ToString();
                    partners.Active = DataHelper.boolParse(row["IsActive"]);
                    partners.Approval = DataHelper.boolParse(row["IsApproval"]);
                    partners.Monetization = DataHelper.intParse(row["TotalMontz"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return partners;
        }

        public bool StatusPartners(int LoginId, int Id, int Status)
        {
            try
            {
                string EncPass = "", Email = "", Password = "", UserName = "";

                if (Status == 1)
                {
                    DataTable dtUser = DataHelper.GetUserDetails(Id);
                    DataRow rowUser = dtUser.Rows[0];

                    Password = PasswordGenerator.Generate();
                    Email = rowUser["UserId"].ToString();
                    UserName = rowUser["UserName"].ToString();
                    EncPass = SBEncryption.getMD5Password(Email, Password);
                }

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status },
                    new SqlParameter { ParameterName = "@Password", Value = EncPass }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("PartnersStatus", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                DataRow row = dt.Rows[0];
                if (row["ErrorCode"].ToString() == "000")
                {
                    if (Status == 1)
                    {
                        string admin_email = ConfigurationManager.AppSettings["admin_email"];

                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/partner_request_approved.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));
                        sbHtml = sbHtml.Replace("{NAME}", UserName);
                        sbHtml = sbHtml.Replace("{EMAIL}", Email);
                        sbHtml = sbHtml.Replace("{PASSWORD}", Password);
                        Emails.SendMail(Email, admin_email, "NetFive - Request Approved.", sbHtml.ToString(), true);
                    }
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public string SavePartner(SmartEcommerce.Models.Common.Partner partner)
        {
            try
            {
                string ProfilePicture = DataHelper.GenerateProfilePicture(partner.FullName);

                #region Categories
                DataTable dTableCategories = new DataTable();
                dTableCategories.Columns.Add("Id", typeof(int));
                dTableCategories.Columns.Add("CategoryId", typeof(string));

                foreach (SmartEcommerce.Models.Product.Category pcTrans in partner.PartnerContentTypeUpload)
                {
                    DataRow new_row = dTableCategories.NewRow();

                    new_row["Id"] = pcTrans.Id;
                    new_row["CategoryId"] = pcTrans.ContentTypeUploadId;

                    dTableCategories.Rows.Add(new_row);
                }
                #endregion

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@FullName", Value = partner.FullName },
                    new SqlParameter { ParameterName = "@PartnerType", Value = partner.PartnerType.Id },
                    new SqlParameter { ParameterName = "@PartnerCategory", Value = partner.PartnerCategory.Id },
                    new SqlParameter { ParameterName = "@ContactPerson", Value = partner.ContactPerson },
                    new SqlParameter { ParameterName = "@Telephone", Value = partner.Telephone },
                    new SqlParameter { ParameterName = "@MobileNo", Value = partner.MobileNo },
                    new SqlParameter { ParameterName = "@EmailAddress", Value = partner.EmailAddress },
                    new SqlParameter { ParameterName = "@Country", Value = partner.Country.Id },
                    new SqlParameter { ParameterName = "@State", Value = partner.State.Id },
                    new SqlParameter { ParameterName = "@City", Value = partner.City.Id },
                    new SqlParameter { ParameterName = "@Address", Value = partner.Address },
                    new SqlParameter { ParameterName = "@PartnerContentType", Value = partner.PartnerContentType.Id },
                    new SqlParameter { ParameterName = "@ProfilePicture", Value = ProfilePicture },
                    new SqlParameter { ParameterName = "@ContentTypeUpload", Value = dTableCategories, SqlDbType = SqlDbType.Structured, TypeName = "type_ContentTypeUpload" },
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("PartnerRequest", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return "999";
                }

                DataRow row = dt.Rows[0];
                if (row["ErrorCode"].ToString() == "000")
                {
                    string admin_email = ConfigurationManager.AppSettings["admin_email"];

                    StringBuilder sbHtml = new StringBuilder();
                    var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/partner_request.html");
                    sbHtml.AppendLine(System.IO.File.ReadAllText(path));
                    sbHtml = sbHtml.Replace("{NAME}", partner.FullName);
                    Emails.SendMail(partner.EmailAddress, admin_email, "NetFive - Want's to Become a Partner!", sbHtml.ToString(), true);
                }

                return row["ErrorCode"].ToString();
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "999";
            }
        }

        public List<SmartEcommerce.Models.Common.PartnerCategory> GetPartnerCategoryByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.PartnerCategory> partnercategory = new List<SmartEcommerce.Models.Common.PartnerCategory>();

                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("PartnerCategoryGetAll", parem, ref response);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        partnercategory.Add(new SmartEcommerce.Models.Common.PartnerCategory()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"])
                        });
                    }
                }

                return partnercategory;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Common.PartnerContentType> GetPartnerContentTypeByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.PartnerContentType> partnercontenttype = new List<SmartEcommerce.Models.Common.PartnerContentType>();

                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("PartnerContentTypeGetAll", parem, ref response);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        partnercontenttype.Add(new SmartEcommerce.Models.Common.PartnerContentType()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"])
                        });
                    }
                }

                return partnercontenttype;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Common.PartnerType> GetPartnerTypeByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Common.PartnerType> partnertype = new List<SmartEcommerce.Models.Common.PartnerType>();

                SqlParameter[] parem = {
                    new SqlParameter { ParameterName = "@Status", Value = Status}
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("PartnerTypeGetAll", parem, ref response);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        partnertype.Add(new SmartEcommerce.Models.Common.PartnerType()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Active = DataHelper.boolParse(row["IsActive"])
                        });
                    }
                }

                return partnertype;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }


        #endregion

        #region Service

        public void ExpireTrialUsers()
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = null;

                DataTable dt = DatabaseObject.FetchTableFromSP("service_expire_trial_users", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }
                else
                {
                    StringBuilder sbHtml = new StringBuilder();
                    var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/trial-expired.html");
                    sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                    string admin_email = ConfigurationManager.AppSettings["admin_email"];

                    foreach (DataRow row in dt.Rows)
                    {
                        string UserId = "";
                        try
                        {
                            UserId = row["UserId"].ToString();


                            string template = sbHtml.ToString();

                            template = template.Replace("{USERNAME}", row["UserName"].ToString());
                            //template = template.Replace("{PRICE}", "$50");

                            bool IsSend = Emails.SendMail(UserId, admin_email, "Your trial is over!", template, true);
                        }
                        catch (Exception ae)
                        {
                            Logs.WriteError("General - " + UserId, ae.Message);
                        }
                    }
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }
        }


        #endregion Service

        #region Payments

        public DataTable GetAllPayments()
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("PaymentGetAll", null, ref response);

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

        public List<SmartEcommerce.Models.Common.Payments> GetPayments()
        {
            try
            {
                List<SmartEcommerce.Models.Common.Payments> payments = new List<SmartEcommerce.Models.Common.Payments>();
                DataTable dt = GetAllPayments();

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        payments.Add(new SmartEcommerce.Models.Common.Payments()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            PaymentId = row["PaymentId"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["Datetime"]),
                            UserId = row["UserId"].ToString(),
                            Description = row["Description"].ToString(),
                            Amount = DataHelper.decimalParse(row["Amount"]),
                            Currency = row["Currency"].ToString(),
                            Status = row["Status"].ToString()
                        });
                    }
                }

                return payments;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        #endregion
    }
}

