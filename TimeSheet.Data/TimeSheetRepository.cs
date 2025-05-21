
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using TimeSheetTest.Data.Interfaces;
using TimeSheetTeste.Models;

namespace TimeSheetTest.Data
{
    public class TimeSheetRepository : BaseRepository, ITimeSheetRepository
    {
        public TimeSheetRepository(DatabaseConnection connection) : base(connection)
        {

        }
        public async Task<bool> InsertTimeSheet(TimeSheetDto dto)  
        {
            bool resultado = false;
            try
            {
                using (var command = _connection.Connection.CreateCommand())
                {
                    int? idParent = null;

                    var taskFather = dto.listTask.First();
                    command.CommandText = @" INSERT INTO TB_TASK (PARENT_TASK_ID, CLIENT_ID, TITLE, PROJECT_ID) VALUES (" + (idParent == null ? "null" : idParent.ToString()) + ", " + taskFather.ClientId + " , '" + taskFather.Title + "', " + dto.ProjectId + " )";

                    var linhasAfetadas = command.ExecuteNonQuery();

                    command.CommandText = @" SELECT MAX(TASK_ID) AS ID_TASK_FATHER FROM TB_TASK";
                    using (var reader = command.ExecuteReader())
                    {
                        var listClient = new List<ClientDto>();

                        while (reader.Read())
                        {
                            idParent = int.Parse(reader["ID_TASK_FATHER"].ToString());
                        }
                    }

                    command.CommandText = @" INSERT INTO TB_TASK_FIELD (TASK_ID, VALUE ) VALUES ((SELECT MAX(TASK_ID) FROM TB_TASK), '" + taskFather.Value + "')";
                    linhasAfetadas = command.ExecuteNonQuery();

                    command.CommandText = @" INSERT INTO TB_CUSTOM_FIELD (FIELD_ID, FIELD_TYPE, FIELD_NAME) VALUES ((SELECT MAX(FIELD_ID) FROM TB_TASK_FIELD), " + taskFather.FieldType + " , '" + taskFather.FieldName + "')";
                    linhasAfetadas = command.ExecuteNonQuery();


                    int idValue = 0;
                    foreach (var task in dto.listTask)
                    {
                        command.CommandText = @" INSERT INTO TB_TASK (PARENT_TASK_ID, CLIENT_ID, TITLE, PROJECT_ID) VALUES (" + ((idValue == 0) ? idParent.ToString() : "(SELECT MAX(TASK_ID) FROM TB_TASK) ") + ", " + task.ClientId + " , '" + task.Title + "'," + dto.ProjectId + " )";
                        linhasAfetadas = command.ExecuteNonQuery();

                        command.CommandText = @" INSERT INTO TB_TASK_FIELD (TASK_ID, VALUE ) VALUES ((SELECT MAX(TASK_ID) FROM TB_TASK), '" + task.Value + "')";
                        linhasAfetadas = command.ExecuteNonQuery();

                        command.CommandText = @" INSERT INTO TB_CUSTOM_FIELD (FIELD_ID, FIELD_TYPE, FIELD_NAME) VALUES ((SELECT MAX(FIELD_ID) FROM TB_TASK_FIELD), " + task.FieldType + " , '" + task.FieldName + "')";
                        linhasAfetadas = command.ExecuteNonQuery();

                        idValue++;
                    }

                    if (linhasAfetadas > 0) resultado = true;


                    command.CommandText = "INSERT INTO TB_TIMESHEET (TASK_ID, USER_ID, TIMESPENT, WORKFED_DATE) VALUES (" + idParent + ", " + dto.UserId + ", " + dto.TimeSpent.TotalMinutes + ", TO_DATE('" + dto.WorkfedDate.ToString() + "','DD/MM/YYYY HH24:MI:SS'))";

                    linhasAfetadas = command.ExecuteNonQuery();
                    if (linhasAfetadas > 0) resultado = true;

                    return resultado;

                }
            }
            catch (Exception EX)
            {
                EX.Message.ToString();
            }

            return resultado;
        }

        public async Task<List<TimeSheetExportDto>> GetTimeSheetExport(TimeSheetExportRequestDto dto)
        {
            using (var command = _connection.Connection.CreateCommand())
            {
               
                string commandText = @"SELECT SUM(TTT.TIMESPENT) AS TOTAL_TIMESPENT ";

                if (dto.ShowWorkFedDate)
                    commandText += @" ,TTT.WORKFED_DATE";

                if (dto.ShowClient)
                    commandText += @",TC.CLIENT_ID, TC.CLIENT_NAME";

                if (dto.ShowProject)
                    commandText += @", TP.PROJECT_ID, TP.PROJECT_NAME";

                commandText += @" FROM TB_TASK  TT INNER JOIN  TB_CLIENT  TC  ON TT.CLIENT_ID = TC.CLIENT_ID ";

                if (dto.ClientId > 0)
                    commandText += @" AND TC.CLIENT_ID = " + dto.ClientId ;

                commandText += @" INNER JOIN TB_PROJECT  TP ON TP.PROJECT_ID = TT.PROJECT_ID ";

                if (dto.ProjectId > 0)
                    commandText += @" AND TT.PROJECT_ID = " + dto.ProjectId;

                commandText += @" INNER JOIN TB_TIMESHEET  TTT ON TTT.TASK_ID = TT.TASK_ID
                        INNER JOIN TB_USER TU ON TU.USER_ID = TTT.USER_ID";

                bool whereContains = false;
                if ((dto.WorkfedDate != null && dto.WorkfedDate != DateTime.MinValue) || dto.ShowProspect)
                    commandText += @" WHERE ";

                if (dto.WorkfedDate != null && dto.WorkfedDate != DateTime.MinValue)
                {
                    commandText += @" TO_DATE(TTT.WORKFED_DATE, 'DD/MM/YY') = TO_DATE('" + dto.WorkfedDate.Value.ToShortDateString() + "', 'DD/MM/YY')";
                    whereContains = true;
                }

                if (dto.ShowProspect)
                {
                    commandText += ( whereContains ? " AND " :  "" ) + @" (
                                          (SELECT 1 FROM TB_TASK_FIELD TTF 
                                                INNER JOIN TB_CUSTOM_FIELD TCF ON TTF.FIELD_ID = TCF.FIELD_ID AND TTF.TASK_ID = TT.TASK_ID  
                    
                                                LEFT JOIN TB_TASK TT2 ON TT2.PARENT_TASK_ID = TT.TASK_ID
                                                LEFT JOIN TB_TASK_FIELD TTF2 ON TTF2.TASK_ID = TT2.TASK_ID
                                                LEFT JOIN TB_CUSTOM_FIELD TC2 ON TC2.FIELD_ID = TTF2.FIELD_ID
                    
                                                LEFT JOIN TB_TASK TT3 ON TT3.PARENT_TASK_ID = TT2.TASK_ID
                                                LEFT JOIN TB_TASK_FIELD TTF3 ON TTF3.TASK_ID = TT3.TASK_ID
                                                LEFT JOIN TB_CUSTOM_FIELD TC3 ON TC3.FIELD_ID = TTF3.FIELD_ID
                    
                                                WHERE ((TCF.FIELD_TYPE = 2 AND TTF.VALUE = 'SIM') OR (TC2.FIELD_TYPE = 2 AND TTF2.VALUE = 'SIM') OR (TC3.FIELD_TYPE = 2 AND TTF3.VALUE = 'SIM')) AND ROWNUM = 1
                                           ) = 1
                                           OR 
                                           (
                                                SELECT 1 FROM TB_TASK_FIELD TTF 
                                                INNER JOIN TB_CUSTOM_FIELD TCF ON TTF.FIELD_ID = TCF.FIELD_ID AND TTF.TASK_ID = TT.TASK_ID 
                    
                                                LEFT JOIN TB_TASK TT2 ON TT2.PARENT_TASK_ID = TT.TASK_ID
                                                LEFT JOIN TB_TASK_FIELD TTF2 ON TTF2.TASK_ID = TT2.TASK_ID
                                                LEFT JOIN TB_CUSTOM_FIELD TC2 ON TC2.FIELD_ID = TTF2.FIELD_ID
                    
                                                LEFT JOIN TB_TASK TT3 ON TT3.PARENT_TASK_ID = TT2.TASK_ID
                                                LEFT JOIN TB_TASK_FIELD TTF3 ON TTF3.TASK_ID = TT3.TASK_ID
                                                LEFT JOIN TB_CUSTOM_FIELD TC3 ON TC3.FIELD_ID = TTF3.FIELD_ID
                    
                                                WHERE ((TCF.FIELD_TYPE = 3 AND TTT.WORKFED_DATE < TO_DATE(TTF.VALUE, 'DD/MM/YYYY HH24:MI:SS'))   OR (TC2.FIELD_TYPE = 3 AND TTT.WORKFED_DATE < TO_DATE(TTF2.VALUE, 'DD/MM/YYYY HH24:MI:SS')) OR (TC3.FIELD_TYPE = 3 AND TTT.WORKFED_DATE < TO_DATE(TTF3.VALUE, 'DD/MM/YYYY HH24:MI:SS')))
                                                AND ROWNUM = 1
                                           ) = 1
                                       ) ";
                }

                if (dto.ShowWorkFedDate || dto.ShowClient || dto.ShowProject)
                    commandText += @" GROUP BY ";

                string commandTextGroup = "";
                if (dto.ShowWorkFedDate)
                    commandTextGroup += @" TTT.WORKFED_DATE";

                if (dto.ShowClient)
                {
                    commandTextGroup += (!string.IsNullOrEmpty(commandTextGroup)) ? ", " : "";
                    commandTextGroup += @" TC.CLIENT_ID, TC.CLIENT_NAME";
                }
                if (dto.ShowProject)
                {
                    commandTextGroup += (!string.IsNullOrEmpty(commandTextGroup)) ? ", " : "";
                    commandTextGroup += @" TP.PROJECT_ID, TP.PROJECT_NAME";
                }

                commandText += commandTextGroup;

                //command.Parameters.Add(new OracleParameter("@ClientId", dto.ClientId));
                //command.Parameters.Add(new OracleParameter("@ProjectId", dto.ProjectId));

                command.CommandText = commandText;
                using (var reader = command.ExecuteReader())
                {
                    var listTimeSheetExport = new List<TimeSheetExportDto>();

                    while (reader.Read())
                    {
                        var hora = TimeSpan.FromMinutes(int.Parse(reader["TOTAL_TIMESPENT"].ToString()));
                        var timeSheetExport = new TimeSheetExportDto
                        {
                            ClientId = (dto.ShowClient) ? int.Parse(reader["CLIENT_ID"].ToString()) : null,
                            ClientName = (dto.ShowClient) ? reader["CLIENT_NAME"].ToString() : null,
                            ProjectId = (dto.ShowProject) ? int.Parse(reader["PROJECT_ID"].ToString()) : null, 
                            ProjectName = (dto.ShowProject) ? reader["PROJECT_NAME"].ToString() : null,
                            WorkfedDate = (dto.ShowWorkFedDate) ? Convert.ToDateTime(reader["WORKFED_DATE"].ToString())  : null,
                            TotalTimeSpent = new TimeSpan(hora.Hours, hora.Minutes, 0).ToString()
                        };

                        listTimeSheetExport.Add(timeSheetExport);
                    }
                    return listTimeSheetExport;
                }
            }
        }

        public List<ClientDto> GetAllClients()
        {
            using (var command = _connection.Connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT CLIENT_ID, CLIENT_NAME FROM TB_CLIENT";

                using (var reader = command.ExecuteReader())
                {
                    var listClient = new List<ClientDto>();

                    while (reader.Read())
                    {
                        var client = new ClientDto
                        {
                            ClientId = int.Parse(reader["CLIENT_ID"].ToString()),
                            ClientName = reader["CLIENT_NAME"].ToString()
                        };

                        listClient.Add(client);
                    }
                    return listClient;
                }
            }
        }

        public List<ProjectDto> GetAllProjects()
        {
            using (var command = _connection.Connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT PROJECT_ID, PROJECT_NAME FROM TB_PROJECT";

                using (var reader = command.ExecuteReader())
                {
                    var listProject = new List<ProjectDto>();

                    while (reader.Read())
                    {
                        var project = new ProjectDto
                        {
                            ProjectId = int.Parse(reader["PROJECT_ID"].ToString()),
                            ProjectName = reader["PROJECT_NAME"].ToString()
                        };

                        listProject.Add(project);
                    }
                    return listProject;
                }
            }
        }

        public List<UserDto> GetAllUsers()
        {
            using (var command = _connection.Connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT USER_ID, USER_NAME FROM TB_USER";

                using (var reader = command.ExecuteReader())
                {
                    var listUser = new List<UserDto>();

                    while (reader.Read())
                    {
                        var user = new UserDto
                        {
                            UserId = int.Parse(reader["USER_ID"].ToString()),
                            UserName = reader["USER_NAME"].ToString()
                        };

                        listUser.Add(user);
                    }
                    return listUser;
                }
            }
        }

    }
}
