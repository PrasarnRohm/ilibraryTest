declare @lotNo as varchar(50) = '1741A3001V'

select 
	lots.id as Id
	,lot_no as [Name]
	,is_automotive as [IsAutomotive]
	,job_id as JobId
	,jobs.[name] as JobName
from trans.lots
	inner join method.device_slips 
		on lots.device_slip_id = device_slips.device_slip_id
	inner join method.device_versions 
		on device_slips.device_id = device_versions.device_id 
		and device_slips.version_num = device_versions.version_num
	inner join method.device_names 
		on device_versions.device_name_id = device_names.id
	inner join method.device_flows 
		on lots.device_slip_id = device_flows.device_slip_id 
		and lots.step_no = device_flows.step_no
	inner join method.jobs 
		on device_flows.job_id = jobs.id
where lot_no = @lotNo