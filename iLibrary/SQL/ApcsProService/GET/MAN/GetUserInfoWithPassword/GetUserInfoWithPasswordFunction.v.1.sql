declare @userCode as varchar(50) = '007294'
declare @password as varchar(50) = '007294'

select 
	id
	,name
	,full_name
	,english_name
	,emp_num
	,default_language 
from man.users 
where users.emp_num = @userCode
	and password = @password