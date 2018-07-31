select 
	lang
	,functionName
	,step
	,stepName 
from dbo.functionStepName 
	inner join dbo.functionName 
		on functionStepName.functionNameId = functionName.id 
order by lang,functionName,step