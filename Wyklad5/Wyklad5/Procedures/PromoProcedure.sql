CREATE PROCEDURE promoProcedure @studies varchar(40), @semester int
AS
BEGIN
	BEGIN TRANSACTION
		DECLARE 
			@lastIdEnroll int,
			@idEnrollPromo int = (
				SELECT IdEnrollment FROM Studies s JOIN Enrollment e ON s.IdStudy = e.IdStudy WHERE e.Semester = @Semester + 1 AND s.Name = @Studies 
			),
			@idEnroll int = (
				SELECT IdEnrollment AS idEnrollment FROM Studies s JOIN Enrollment e ON s.IdStudy = e.IdStudy WHERE e.Semester = @Semester AND s.Name = @Studies
			)
		IF (@idEnrollPromo IS NULL)
			BEGIN
				SET @idEnrollPromo = @lastIdEnroll + 1
				SET @lastIdEnroll = (SELECT MAX(IdEnrollment) FROM Enrollment)
				INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@idEnrollPromo, @Semester + 1, (SELECT IdStudy FROM Studies WHERE Name = @Studies), GETDATE())
			END
		UPDATE Student 
		SET IdEnrollment = @idEnrollPromo WHERE @idEnroll = IdEnrollment
		SELECT IdEnrollment, e.IdStudy, Semester, StartDate FROM Enrollment e JOIN Studies s ON e.IdStudy = s.IdStudy WHERE IdEnrollment = @idEnrollPromo;
		COMMIT
END