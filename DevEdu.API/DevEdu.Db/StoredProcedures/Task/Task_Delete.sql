﻿CREATE PROCEDURE dbo.Task_Delete
	@Id int
AS
	UPDATE dbo.Task
    SET
    IsDeleted = 1
    WHERE Id = @Id