declare @machineId as int = 2

select 
	is_disabled 
from mc.machines 
where id = @machineId

select 
	run_state
	,qc_state 
from [trans].machine_states 
where machine_id = @machineId

