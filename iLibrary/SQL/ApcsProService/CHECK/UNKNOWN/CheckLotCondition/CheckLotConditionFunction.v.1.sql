declare @lotId as varchar(50) = '1'

select 
	process_state 
from [trans].lots 
where id = @lotId

