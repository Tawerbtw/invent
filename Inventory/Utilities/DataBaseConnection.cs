using Inventory.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace Inventory.Utilities
{
    public class DataBaseConnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string username;
        private string password;
        private string port;

        public DataBaseConnection()
        {
            server = "127.0.0.1";

            database = "qrwe";
            username = "root";
            password = "";
            port = "3307";
            string connectionString = $"Server={server};Port={port};Database={database};User ID={username};Password={password};";

            connection = new MySqlConnection(connectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                // Обработка ошибок подключения
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                // Обработка ошибок закрытия соединения
                return false;
            }
        }


        public ObservableCollection<EquipmentModel> LoadEquipmentData()
        {
            ObservableCollection<EquipmentModel> equipmentList = new ObservableCollection<EquipmentModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Equipment";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EquipmentModel equipment = new EquipmentModel
                        {
                            Id = Convert.ToInt32(reader["EquipmentID"]),
                            EquipmentName = reader["EquipmentName"].ToString(),
                            EquipmentImage = reader["EquipmentImage"] as byte[], // Прямо из reader
                            InventoryNumber = reader["InventoryNumber"].ToString(),
                            Auditorium = reader["Auditorium"].ToString(),
                            ResponsibleUser = reader["ResponsibleUser"].ToString(),
                            TemporaryUser = reader["TemporaryUser"].ToString(),
                            EquipmentCost = Convert.ToDecimal(reader["EquipmentCost"]),
                            EquipmentDirection = reader["EquipmentDirection"].ToString(),
                            EquipmentStatus = reader["EquipmentStatus"].ToString(),
                            EquipmentType = reader["EquipmentType"].ToString(),
                            EquipmentComment = reader["EquipmentComment"].ToString()
                        };
                        equipmentList.Add(equipment);
                    }
                }

                CloseConnection();
            }

            return equipmentList;
        }

        public bool AddEquipment(EquipmentModel equipment)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO Equipment (EquipmentName, EquipmentImage, InventoryNumber, Auditorium, ResponsibleUser, TemporaryUser, EquipmentCost, EquipmentDirection, EquipmentStatus, EquipmentType, EquipmentComment) " +
                               "VALUES (@EquipmentName, @EquipmentImage, @InventoryNumber, @Auditorium, @ResponsibleUser, @TemporaryUser, @EquipmentCost, @EquipmentDirection, @EquipmentStatus, @EquipmentType, @EquipmentComment)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EquipmentName", equipment.EquipmentName);
                cmd.Parameters.AddWithValue("@EquipmentImage", equipment.EquipmentImage);
                cmd.Parameters.AddWithValue("@InventoryNumber", equipment.InventoryNumber);
                cmd.Parameters.AddWithValue("@Auditorium", equipment.Auditorium);
                cmd.Parameters.AddWithValue("@ResponsibleUser", equipment.ResponsibleUser);
                cmd.Parameters.AddWithValue("@TemporaryUser", equipment.TemporaryUser);
                cmd.Parameters.AddWithValue("@EquipmentCost", equipment.EquipmentCost);
                cmd.Parameters.AddWithValue("@EquipmentDirection", equipment.EquipmentDirection);
                cmd.Parameters.AddWithValue("@EquipmentStatus", equipment.EquipmentStatus);
                cmd.Parameters.AddWithValue("@EquipmentType", equipment.EquipmentType);
                cmd.Parameters.AddWithValue("@EquipmentComment", equipment.EquipmentComment);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateEquipment(EquipmentModel equipment)
        {
            if (OpenConnection())
            {
                string query = "UPDATE Equipment SET EquipmentName = @EquipmentName, EquipmentImage = @EquipmentImage, InventoryNumber = @InventoryNumber, " +
                               "Auditorium = @Auditorium, ResponsibleUser = @ResponsibleUser, TemporaryUser = @TemporaryUser, " +
                               "EquipmentCost = @EquipmentCost, EquipmentDirection = @EquipmentDirection, EquipmentStatus = @EquipmentStatus, " +
                               "EquipmentType = @EquipmentType, EquipmentComment = @EquipmentComment " +
                               "WHERE EquipmentID = @EquipmentID";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EquipmentID", equipment.Id);
                cmd.Parameters.AddWithValue("@EquipmentName", equipment.EquipmentName);
                cmd.Parameters.AddWithValue("@EquipmentImage", equipment.EquipmentImage);
                cmd.Parameters.AddWithValue("@InventoryNumber", equipment.InventoryNumber);
                cmd.Parameters.AddWithValue("@Auditorium", equipment.Auditorium);
                cmd.Parameters.AddWithValue("@ResponsibleUser", equipment.ResponsibleUser);
                cmd.Parameters.AddWithValue("@TemporaryUser", equipment.TemporaryUser);
                cmd.Parameters.AddWithValue("@EquipmentCost", equipment.EquipmentCost);
                cmd.Parameters.AddWithValue("@EquipmentDirection", equipment.EquipmentDirection);
                cmd.Parameters.AddWithValue("@EquipmentStatus", equipment.EquipmentStatus);
                cmd.Parameters.AddWithValue("@EquipmentType", equipment.EquipmentType);
                cmd.Parameters.AddWithValue("@EquipmentComment", equipment.EquipmentComment);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteEquipment(int equipmentID)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "DELETE FROM Equipment WHERE EquipmentID = @EquipmentID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@EquipmentID", equipmentID);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<AuditoriumModel> LoadAuditoriumData()
        {
            ObservableCollection<AuditoriumModel> auditoriumList = new ObservableCollection<AuditoriumModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Auditorium";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AuditoriumModel auditorium = new AuditoriumModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            ShortName = reader["ShortName"].ToString(),
                            ResponsibleUser = reader["ResponsibleUser"].ToString(),
                            TemporaryUser = reader["TemporaryUser"].ToString()
                        };
                        auditoriumList.Add(auditorium);
                    }
                }

                CloseConnection();
            }

            return auditoriumList;
        }

        public bool AddAuditorium(AuditoriumModel auditorium)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "INSERT INTO Auditorium (Name, ShortName, ResponsibleUser, TemporaryUser) " +
                                   "VALUES (@Name, @ShortName, @ResponsibleUser, @TemporaryUser)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Name", auditorium.Name);
                    cmd.Parameters.AddWithValue("@ShortName", auditorium.ShortName);
                    cmd.Parameters.AddWithValue("@ResponsibleUser", auditorium.ResponsibleUser);
                    cmd.Parameters.AddWithValue("@TemporaryUser", auditorium.TemporaryUser);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Этот пользователь занят");
                    return false;
                }
            }

            return false;
        }

        public bool UpdateAuditorium(AuditoriumModel auditorium)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "UPDATE Auditorium SET Name = @Name, ShortName = @ShortName, ResponsibleUser = @ResponsibleUser, TemporaryUser = @TemporaryUser " +
                                   "WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", auditorium.Id);
                    cmd.Parameters.AddWithValue("@Name", auditorium.Name);
                    cmd.Parameters.AddWithValue("@ShortName", auditorium.ShortName);
                    cmd.Parameters.AddWithValue("@ResponsibleUser", auditorium.ResponsibleUser);
                    cmd.Parameters.AddWithValue("@TemporaryUser", auditorium.TemporaryUser);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                        MessageBox.Show("Этот пользователь занят");
                        return false;
                }
            }

            return false;
        }
        public bool DeleteAuditorium(int auditoriumId)
        {
        if (OpenConnection())
        {
            try
            {
                string query = "DELETE FROM Auditorium WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", auditoriumId);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                    MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                return false;
                }
            }

            return false;
         }


    public ObservableCollection<DirectionModel> LoadDirectionData()
        {
            ObservableCollection<DirectionModel> directionList = new ObservableCollection<DirectionModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Direction";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DirectionModel direction = new DirectionModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        };
                        directionList.Add(direction);
                    }
                }

                CloseConnection();
            }

            return directionList;
        }

        public bool AddDirection(DirectionModel direction)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO Direction (Name) " +
                               "VALUES (@Name)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", direction.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateDirection(DirectionModel direction)
        {
            if (OpenConnection())
            {
                string query = "UPDATE Direction SET Name = @Name " +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", direction.Id);
                cmd.Parameters.AddWithValue("@Name", direction.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteDirection(int directionId)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "DELETE FROM Direction WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", directionId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }
            return false;
        }

        public ObservableCollection<StatusModel> LoadStatusData()
        {
            ObservableCollection<StatusModel> statusList = new ObservableCollection<StatusModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Status";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StatusModel status = new StatusModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        };
                        statusList.Add(status);
                    }
                }

                CloseConnection();
            }

            return statusList;
        }

        public bool AddStatus(StatusModel status)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO Status (Name) " +
                               "VALUES (@Name)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", status.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateStatus(StatusModel status)
        {
            if (OpenConnection())
            {
                string query = "UPDATE Status SET Name = @Name " +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", status.Id);
                cmd.Parameters.AddWithValue("@Name", status.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteStatus(int statusId)
        {
            if (OpenConnection())
            {
                try
                {
                    string query = "DELETE FROM Status WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", statusId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<EquipmentTypeModel> LoadEquipmentTypesData()
        {
            ObservableCollection<EquipmentTypeModel> equipmentTypesList = new ObservableCollection<EquipmentTypeModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM EquipmentType";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EquipmentTypeModel equipmentType = new EquipmentTypeModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        };
                        equipmentTypesList.Add(equipmentType);
                    }
                }

                CloseConnection();
            }

            return equipmentTypesList;
        }

        public bool AddEquipmentType(EquipmentTypeModel equipmentType)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO EquipmentType (Name) " +
                               "VALUES (@Name)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", equipmentType.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateEquipmentType(EquipmentTypeModel equipmentType)
        {
            if (OpenConnection())
            {
                string query = "UPDATE EquipmentType SET Name = @Name " +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", equipmentType.Id);
                cmd.Parameters.AddWithValue("@Name", equipmentType.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteEquipmentType(int equipmentTypeID)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "DELETE FROM EquipmentType WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", equipmentTypeID);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }
            return false;
        }

        public ObservableCollection<ModelTypeModel> LoadModelTypeData()
        {
            ObservableCollection<ModelTypeModel> modelTypeList = new ObservableCollection<ModelTypeModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM ModelType";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ModelTypeModel modelType = new ModelTypeModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            EquipmentTypeCode = reader["EquipmentTypeCode"].ToString()
                        };
                        modelTypeList.Add(modelType);
                    }
                }

                CloseConnection();
            }

            return modelTypeList;
        }

        public bool AddModelType(ModelTypeModel modelType)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO ModelType (Name, EquipmentTypeCode) " +
                               "VALUES (@Name, @EquipmentTypeCode)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", modelType.Name);
                cmd.Parameters.AddWithValue("@EquipmentTypeCode", modelType.EquipmentTypeCode);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateModelType(ModelTypeModel modelType)
        {
            if (OpenConnection())
            {
                string query = "UPDATE ModelType SET Name = @Name, EquipmentTypeCode = @EquipmentTypeCode " +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", modelType.Id);
                cmd.Parameters.AddWithValue("@Name", modelType.Name);
                cmd.Parameters.AddWithValue("@EquipmentTypeCode", modelType.EquipmentTypeCode);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteModelType(int modelTypeId)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "DELETE FROM ModelType WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", modelTypeId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<ProgramModel> LoadProgramData()
        {
            ObservableCollection<ProgramModel> programList = new ObservableCollection<ProgramModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Program";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProgramModel program = new ProgramModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Developer = reader["Developer"].ToString(),
                            Version = reader["Version"].ToString()
                        };
                        programList.Add(program);
                    }
                }

                CloseConnection();
            }

            return programList;
        }

        public bool AddProgram(ProgramModel program)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO Program (Name, Developer, Version) " +
                               "VALUES (@Name, @Developer, @Version)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", program.Name);
                cmd.Parameters.AddWithValue("@Developer", program.Developer);
                cmd.Parameters.AddWithValue("@Version", program.Version);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateProgram(ProgramModel program)
        {
            if (OpenConnection())
            {
                string query = "UPDATE Program SET Name = @Name, Developer = @Developer , Version = @Version" +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", program.Id);
                cmd.Parameters.AddWithValue("@Name", program.Name);
                cmd.Parameters.AddWithValue("@Developer", program.Developer);
                cmd.Parameters.AddWithValue("@Version", program.Version);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteProgram(int programId)
        {
            if (OpenConnection())
            {
                try
                {
                    string query = "DELETE FROM Program WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", programId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<DeveloperModel> LoadDeveloperData()
        {
            ObservableCollection<DeveloperModel> developerList = new ObservableCollection<DeveloperModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Developer";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DeveloperModel developer = new DeveloperModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        };
                        developerList.Add(developer);
                    }
                }

                CloseConnection();
            }

            return developerList;
        }

        public bool AddDeveloper(DeveloperModel developer)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO Developer (Name) " +
                               "VALUES (@Name)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", developer.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateDeveloper(DeveloperModel developer)
        {
            if (OpenConnection())
            {
                string query = "UPDATE Developer SET Name = @Name " +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", developer.Id);
                cmd.Parameters.AddWithValue("@Name", developer.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteDeveloper(int developerID)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "DELETE FROM Developer WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", developerID);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<InventoryModel> LoadInventoryData()
        {
            ObservableCollection<InventoryModel> inventoryList = new ObservableCollection<InventoryModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Inventory";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        InventoryModel inventory = new InventoryModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = Convert.ToDateTime(reader["EndDate"]),
                            Name = reader["Name"].ToString(),
                            InventoriedEquipment = reader["InventoriedEquipment"].ToString(),
                            InventoryComment = reader["InventoryComment"].ToString(),
                            AuthorizedUser = reader["AuthorizedUser"].ToString()
                        };
                        inventoryList.Add(inventory);
                    }
                }

                CloseConnection();
            }

            return inventoryList;
        }

        public bool AddInventory(InventoryModel inventory)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO Inventory (StartDate, EndDate, Name, InventoriedEquipment, InventoryComment, AuthorizedUser) " +
                               "VALUES (@StartDate, @EndDate, @Name, @InventoriedEquipment, @InventoryComment, @AuthorizedUser)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@StartDate", inventory.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", inventory.EndDate);
                cmd.Parameters.AddWithValue("@Name", inventory.Name);
                cmd.Parameters.AddWithValue("@InventoriedEquipment", inventory.InventoriedEquipment);
                cmd.Parameters.AddWithValue("@InventoryComment", inventory.InventoryComment);
                cmd.Parameters.AddWithValue("@AuthorizedUser", inventory.AuthorizedUser);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateInventory(InventoryModel inventory)
        {
            if (OpenConnection())
            {
                string query = "UPDATE Inventory SET StartDate = @StartDate, EndDate = @EndDate, Name = @Name, " +
                               "InventoriedEquipment = @InventoriedEquipment, InventoryComment = @InventoryComment, " +
                               "AuthorizedUser = @AuthorizedUser " +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", inventory.Id);
                cmd.Parameters.AddWithValue("@StartDate", inventory.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", inventory.EndDate);
                cmd.Parameters.AddWithValue("@Name", inventory.Name);
                cmd.Parameters.AddWithValue("@InventoriedEquipment", inventory.InventoriedEquipment);
                cmd.Parameters.AddWithValue("@InventoryComment", inventory.InventoryComment);
                cmd.Parameters.AddWithValue("@AuthorizedUser", inventory.AuthorizedUser);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteInventory(int id)
        {
            if (OpenConnection())
            {
                try
                {
                    string query = "DELETE FROM Inventory WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<UserModel> users = new ObservableCollection<UserModel>();

        public ObservableCollection<UserModel> LoadUserData()
        {
            ObservableCollection<UserModel> userList = new ObservableCollection<UserModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM User";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserModel user = new UserModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Login = reader["Login"].ToString(),
                            Password = reader["Password"].ToString(),
                            Role = reader["Role"].ToString(),
                            Email = reader["Email"].ToString(),
                            SecondName = reader["SecondName"].ToString(),
                            Name = reader["Name"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Address = reader["Address"].ToString()
                        };
                        userList.Add(user);
                    }
                }

                CloseConnection();
            }

            users = userList; // Сохраняем загруженные пользователи

            return userList;
        }


        public bool AddUser(UserModel user)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO User (Login, Password, Role, Email, SecondName, Name, LastName, Phone, Address) " +
                               "VALUES (@Login, @Password, @Role, @Email, @SecondName, @Name, @LastName, @Phone, @Address)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Login", user.Login);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@SecondName", user.SecondName);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Phone", user.Phone);
                cmd.Parameters.AddWithValue("@Address", user.Address);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateUser(UserModel user)
        {
            if (OpenConnection())
            {
                string query = "UPDATE User SET Login = @Login, Password = @Password, Role = @Role, " +
                               "Email = @Email, SecondName = @SecondName, Name = @Name, LastName = @LastName, " +
                               "Phone = @Phone, Address = @Address " +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", user.Id);
                cmd.Parameters.AddWithValue("@Login", user.Login);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@SecondName", user.SecondName);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Phone", user.Phone);
                cmd.Parameters.AddWithValue("@Address", user.Address);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteUser(int userId)
        {
            if (OpenConnection())
            {
                try
                {
                    string query = "DELETE FROM User WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<NetworkModel> LoadNetworkData()
        {
            ObservableCollection<NetworkModel> networkList = new ObservableCollection<NetworkModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Network";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NetworkModel network = new NetworkModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Ip = reader["Ip"].ToString(),
                            Mask = reader["Mask"].ToString(),
                            Gateway = reader["Gateway"].ToString(),
                            Dns = reader["Dns"].ToString()
                        };
                        networkList.Add(network);
                    }
                }

                CloseConnection();
            }

            return networkList;
        }

        public bool AddNetwork(NetworkModel network)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO Network (Ip, Mask, Gateway, Dns) " +
                               "VALUES (@Ip, @Mask, @Gateway, @Dns)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Ip", network.Ip);
                cmd.Parameters.AddWithValue("@Mask", network.Mask);
                cmd.Parameters.AddWithValue("@Gateway", network.Gateway);
                cmd.Parameters.AddWithValue("@Dns", network.Dns);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateNetwork(NetworkModel network)
        {
            if (OpenConnection())
            {
                string query = "UPDATE Network SET Ip = @Ip, Mask = @Mask, Gateway = @Gateway, " +
                               "Dns = @Dns WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", network.Id);
                cmd.Parameters.AddWithValue("@Ip", network.Ip);
                cmd.Parameters.AddWithValue("@Mask", network.Mask);
                cmd.Parameters.AddWithValue("@Gateway", network.Gateway);
                cmd.Parameters.AddWithValue("@Dns", network.Dns);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteNetwork(int networkId)
        {
            if (OpenConnection())
            {
                try
                {
                    string query = "DELETE FROM Network WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", networkId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<MaterialsModel> LoadMaterialsData()
        {
            ObservableCollection<MaterialsModel> materialsList = new ObservableCollection<MaterialsModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM Materials";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MaterialsModel material = new MaterialsModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            DateAdd = Convert.ToDateTime(reader["DateAdd"]),
                            Image = reader["Image"] as byte[],
                            Count = Convert.ToInt32(reader["Count"]),
                            ResponsibleUser = reader["ResponsibleUser"].ToString(),
                            TemporaryUser = reader["TemporaryUser"].ToString(),
                            Type = reader["Type"].ToString()
                        };
                        materialsList.Add(material);
                    }
                }

                CloseConnection();
            }

            return materialsList;
        }

        public bool AddMaterial(MaterialsModel material)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO Materials (Name, Description, DateAdd, Image, Count, ResponsibleUser, TemporaryUser, Type) " +
                               "VALUES (@Name, @Description, @DateAdd, @Image, @Count, @ResponsibleUser, @TemporaryUser, @Type)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", material.Name);
                cmd.Parameters.AddWithValue("@Description", material.Description);
                cmd.Parameters.AddWithValue("@DateAdd", material.DateAdd);
                cmd.Parameters.AddWithValue("@Image", material.Image);
                cmd.Parameters.AddWithValue("@Count", material.Count);
                cmd.Parameters.AddWithValue("@ResponsibleUser", material.ResponsibleUser);
                cmd.Parameters.AddWithValue("@TemporaryUser", material.TemporaryUser);
                cmd.Parameters.AddWithValue("@Type", material.Type);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateMaterial(MaterialsModel material)
        {
            if (OpenConnection())
            {
                string query = "UPDATE Materials SET Name = @Name, Description = @Description,DateAdd = @DateAdd , Image = @Image, " +
                               "Count = @Count, ResponsibleUser = @ResponsibleUser, TemporaryUser = @TemporaryUser, " +
                               "Type = @Type WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", material.Id);
                cmd.Parameters.AddWithValue("@Name", material.Name);
                cmd.Parameters.AddWithValue("@Description", material.Description);
                cmd.Parameters.AddWithValue("@DateAdd", material.DateAdd);
                cmd.Parameters.AddWithValue("@Image", material.Image);
                cmd.Parameters.AddWithValue("@Count", material.Count);
                cmd.Parameters.AddWithValue("@ResponsibleUser", material.ResponsibleUser);
                cmd.Parameters.AddWithValue("@TemporaryUser", material.TemporaryUser);
                cmd.Parameters.AddWithValue("@Type", material.Type);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteMaterial(int materialId)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "DELETE FROM Materials WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", materialId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<TypeMaterialModel> LoadMaterialTypesData()
        {
            ObservableCollection<TypeMaterialModel> materialTypeList = new ObservableCollection<TypeMaterialModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM TypeMaterial";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TypeMaterialModel materialType = new TypeMaterialModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        };
                        materialTypeList.Add(materialType);
                    }
                }

                CloseConnection();
            }

            return materialTypeList;
        }

        public bool AddMaterialType(TypeMaterialModel materialType)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO TypeMaterial (Name) VALUES (@Name)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", materialType.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateMaterialType(TypeMaterialModel materialType)
        {
            if (OpenConnection())
            {
                string query = "UPDATE TypeMaterial SET Name = @Name WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", materialType.Id);
                cmd.Parameters.AddWithValue("@Name", materialType.Name);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteMaterialType(int materialTypeID)
        {
            if (OpenConnection())
            {
                try
                {


                    string query = "DELETE FROM TypeMaterial WHERE Id = @Id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", materialTypeID);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }

        public ObservableCollection<CharConsumbalesModel> LoadCharConsumablesData()
        {
            ObservableCollection<CharConsumbalesModel> charConsumablesList = new ObservableCollection<CharConsumbalesModel>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM CharConsumables";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CharConsumbalesModel charConsumable = new CharConsumbalesModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CharName = reader["CharName"].ToString(),
                            Consumbale = reader["Consumable"].ToString()
                        };
                        charConsumablesList.Add(charConsumable);
                    }
                }

                CloseConnection();
            }

            return charConsumablesList;
        }

        public bool AddCharConsumable(CharConsumbalesModel charConsumable)
        {
            if (OpenConnection())
            {
                string query = "INSERT INTO CharConsumables (CharName, Consumable) " +
                               "VALUES (@CharName, @Consumable)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@CharName", charConsumable.CharName);
                cmd.Parameters.AddWithValue("@Consumable", charConsumable.Consumbale);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool UpdateCharConsumable(CharConsumbalesModel charConsumable)
        {
            if (OpenConnection())
            {
                string query = "UPDATE CharConsumables SET CharName = @CharName, Consumable = @Consumable " +
                               "WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", charConsumable.Id);
                cmd.Parameters.AddWithValue("@CharName", charConsumable.CharName);
                cmd.Parameters.AddWithValue("@Consumable", charConsumable.Consumbale);

                int rowsAffected = cmd.ExecuteNonQuery();

                CloseConnection();

                return rowsAffected > 0;
            }

            return false;
        }

        public bool DeleteCharConsumable(int charConsumableId)
        {
            if (OpenConnection())
            {
                try
                {
                    string query = "DELETE FROM CharConsumables WHERE Id = @Id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", charConsumableId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    CloseConnection();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как на нее ссылаются в таблице equipment.", "Ошибка удаления");
                        return false;
                    }
                    return false;
                }
            }

            return false;
        }
        public ObservableCollection<HistoryAud> LoadhistoryAuds()
        {
            ObservableCollection<HistoryAud> programList = new ObservableCollection<HistoryAud>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM HistoryAud";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HistoryAud program = new HistoryAud
                        {
                            EquipmentName = reader["InventoryNumber"].ToString(),
                            InventoryNumber = reader["Name"].ToString(),
                            Auditorium = reader["Developer"].ToString(),
                        };
                        programList.Add(program);
                    }
                }

                CloseConnection();
            }
            return programList;
        }
        public ObservableCollection<HistoryUser> LoadHistoryUser()
        {
            ObservableCollection<HistoryUser> programList = new ObservableCollection<HistoryUser>();

            if (OpenConnection())
            {
                string query = "SELECT * FROM HistoryUser";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HistoryUser program = new HistoryUser
                        {
                            InventoryNumber =reader["InventoryNumber"].ToString(),
                            ResponsibleUser = reader["Name"].ToString(),
                            EquipmentComment = reader["Developer"].ToString(),
                        };
                        programList.Add(program);
                    }
                }

                CloseConnection();
            }

            return programList;
        }
    }
}