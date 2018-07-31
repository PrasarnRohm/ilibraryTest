declare @employee as varchar(50) = '007294'
declare @appName as varchar(50) = 'CellCon_DB'
declare @functionName as varchar(50) = 'Setup'
select * from man.users inner join man.user_roles on [user_id] = users.ID 
inner join man.role_permissions on user_roles.role_id = role_permissions.role_id 
inner join man.permission_operations on role_permissions.permission_id = permission_operations.permission_id 
inner join man.operations on permission_operations.operation_id = operations.id 
where users.emp_num = @employee and [app_name] = @appName and function_name = @functionName and lockout = 0
