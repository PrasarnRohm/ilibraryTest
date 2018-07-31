select 
	lang
	,code
	,[message]
from mdm.errors 
where app_name = 'iLibrary' 
order by lang,code