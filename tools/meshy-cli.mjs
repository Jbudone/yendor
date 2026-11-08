import { meshBalance, meshStatus, submitPreview } from './meshy.mjs';

try {
  const [command, value] = process.argv.slice(2);
  let result;
  if (command === 'balance') result = await meshBalance();
  else if (command === 'status') result = await meshStatus(value);
  else if (command === 'preview' && value) result = await submitPreview(value);
  else throw new Error('Usage: npm run meshy -- balance | status TASK_ID | preview content/art/FILE.json');
  console.log(JSON.stringify(result, null, 2));
} catch (error) { console.error(error.message); process.exitCode = 1; }
