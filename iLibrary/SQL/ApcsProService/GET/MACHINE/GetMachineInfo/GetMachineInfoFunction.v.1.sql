declare @machineName as varchar(50) = 'DB-AS-01'

select 
	machines.id as Id
	,machines.[name] as [Name]
	,models.id as MachineTypeId
	,models.[name] as MachineType
	,is_automotive as IsAutomotive
from mc.machines 
	inner join mc.models on machine_model_id = models.id
where machines.[name] = @machineName