﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


namespace DevEdu.DAL.Repositories
{
    public class TagRepository : BaseRepository
    {
        public int InsertTagToTagMaterial(int materialId, int tagId)
        {
            return _connection
                .QuerySingle(
                "[dbo].[Tag_Material_Insert]",
                new { tagId , materialId }, 
                commandType: CommandType.StoredProcedure
                );
        }
        public void DeleteTagFromTagMaterial(int materialId, int tagId)
        {
            _connection
                .Execute("[dbo].[Tag_Material_Delete]",
                new { tagId , materialId}, 
                commandType: CommandType.StoredProcedure
                );
        }
        public int InsertTagToTagTask(int taskId, int tagId)
        {
            return _connection
                .QuerySingle("[dbo].[Tag_Task_Insert]",
                new { tagId, taskId },
                commandType: CommandType.StoredProcedure
                );
        }
        public void DeleteTagFromTagTask(int taskId, int tagId)
        {
            _connection
                .Execute("[Tag_Task_Delete]", 
                new { tagId, taskId },
                commandType: CommandType.StoredProcedure
                );
        }
    }
}
