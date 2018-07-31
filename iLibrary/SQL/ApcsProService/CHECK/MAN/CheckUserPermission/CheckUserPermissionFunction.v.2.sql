declare @userID as varchar(50) = '1135'
declare @appName as varchar(50) = 'CellCon_DB'
declare @functionName as varchar(50) = 'Setup'
select case when user_roles.expired_on > getdate() then 1 end as LicenseExpired from man.user_roles
inner join man.role_permissions on user_roles.role_id = role_permissions.role_id
inner join man.permission_operations on role_permissions.permission_id = permission_operations.permission_id 
inner join man.operations on permission_operations.operation_id = operations.id
where [user_id] = @userID and [app_name] = @appName and function_name = @functionName
