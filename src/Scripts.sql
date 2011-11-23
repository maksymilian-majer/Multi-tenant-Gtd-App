CREATE DATABASE [gtdapp]
GO

USE [gtdapp]
GO

CREATE TABLE Tenants (
	TenantId UNIQUEIDENTIFIER NOT NULL,
	Subdomain VARCHAR(50) NOT NULL
	CONSTRAINT PK_Tenants PRIMARY KEY CLUSTERED
	(
		TenantId
	)
)
GO

CREATE PROCEDURE dbo.GetTenantIdBySubdomain
(
	@subdomain VARCHAR(50)
)
AS
BEGIN
	SELECT TenantId FROM Tenants WHERE Subdomain = @subdomain
END
GO

CREATE FUNCTION GetCurrentTenantID()
RETURNS UNIQUEIDENTIFIER
AS
BEGIN
	RETURN CAST(CAST(CONTEXT_INFO() AS BINARY(16)) AS UNIQUEIDENTIFIER)
END
GO
 
CREATE PROCEDURE SetCurrentTenantID(@tenantId UNIQUEIDENTIFIER)
AS
BEGIN
  SET CONTEXT_INFO @tenantId
END
GO

CREATE TABLE Contexts (
	TenantId	UNIQUEIDENTIFIER NOT NULL,
	ContextId	UNIQUEIDENTIFIER NOT NULL,
	Name		VARCHAR(50) NOT NULL,
	CONSTRAINT PK_Contexts PRIMARY KEY NONCLUSTERED
	(
		ContextId
	),
	CONSTRAINT FK_Contexts_Tenants FOREIGN KEY
	(
		TenantId
	) REFERENCES Tenants (TenantId),
)
GO

CREATE UNIQUE CLUSTERED INDEX IX_TenantId_ContextId ON Contexts
(
	TenantId,
	ContextId
)
GO

ALTER TABLE Contexts ADD CONSTRAINT CN_Contexts_TenantId  DEFAULT (dbo.GetCurrentTenantID()) FOR TenantId
GO

CREATE TABLE Todos (
	TenantId	UNIQUEIDENTIFIER NOT NULL,
	TodoId		UNIQUEIDENTIFIER NOT NULL,
	ContextId	UNIQUEIDENTIFIER NOT NULL,
	Name		VARCHAR(500) NOT NULL,
	IsDone		BIT NOT NULL,
	CONSTRAINT PK_Todos PRIMARY KEY NONCLUSTERED
	(
		TodoId
	),
	CONSTRAINT FK_Todos_Tenants FOREIGN KEY
	(
		TenantId
	) REFERENCES Tenants (TenantId),
	CONSTRAINT FK_Todos_Contexts FOREIGN KEY
	(
		TenantId,
		ContextId
	) REFERENCES Contexts (TenantId, ContextId)
)
GO

CREATE UNIQUE CLUSTERED INDEX IX_TenantId_TodoId ON Todos
(
	TenantId,
	TodoId
)
GO

ALTER TABLE Todos ADD CONSTRAINT CN_Todos_TenantId  DEFAULT (dbo.GetCurrentTenantID()) FOR TenantId
GO

CREATE VIEW TenantContextsView AS 
 SELECT 
	 * 
 FROM 
	 Contexts
 WHERE 
	 TenantId = dbo.GetCurrentTenantID()
 WITH CHECK OPTION
GO

CREATE VIEW TenantTodosView AS 
 SELECT 
	 * 
 FROM 
	 Todos
 WHERE 
	 TenantId = dbo.GetCurrentTenantID()
 WITH CHECK OPTION
GO

CREATE SCHEMA tenancy
GO

CREATE VIEW tenancy.Contexts AS
	SELECT
		ContextId	,
		Name		
	FROM
		dbo.TenantContextsView
GO

CREATE VIEW tenancy.Todos AS
	SELECT
		TodoId		,
		ContextId	,
		Name		,
		IsDone		
	FROM
		dbo.TenantTodosView
GO

INSERT INTO [dbo].[Tenants] VALUES (NewId(), 'tenant1')
INSERT INTO [dbo].[Tenants] VALUES (NewId(), 'tenant2')
GO