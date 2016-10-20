local key = @key;
local value = tonumber(@value);

local min;
local max;
local count;
local sum;

local values = redis.call('HMGET', key, 'min', 'max', 'count', 'sum');

if(tonumber(values[1]) == nil) then
    min = value;
    max = value;
    count = 1;
    sum = value;
else
    min = math.min(value, tonumber(values[1]));
    max = math.max(value, tonumber(values[2]));
    count = tonumber(values[3]) + 1;
    sum = tonumber(values[4]) + value;
end;

local mean;

if(count > 1) then
    mean = sum / count;
else
    mean = value;
end;

redis.call('HMSET', key, 'min', min, 'max', max, 'current', value, 'mean', mean, 'count', count, 'sum', sum);
