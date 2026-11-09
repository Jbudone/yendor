export const roles = {
  combat: {
    description: 'Implements third-person movement, gunplay, enemy behavior, damage and combat readability. Returns changed paths and validation needs.',
    scopes: ['game/Assets/Yendor/Runtime/Combat'],
    focus: 'Keep inputs responsive and combat deterministic. Preserve readable attack windows. Prioritize movement feel and clear hit feedback.'
  },
  systems: {
    description: 'Implements interacting roguelike items, statuses, hazards, inventory and run rules. Returns interaction examples and changed paths.',
    scopes: ['game/Assets/Yendor/Runtime/Systems'],
    focus: 'Prefer reusable rules and explicit interactions. Explain player discovery and counterplay. Keep seed replay reproducible.'
  },
  world: {
    description: 'Builds seeded expedition routes, exploration landmarks and reproducible Unity scene builders. Returns changed paths and traversal checks.',
    scopes: ['game/Assets/Yendor/Runtime/World', 'game/Assets/Yendor/Editor'],
    focus: 'Protect traversability, spawn safety, sightlines and exploration pacing. Change scenes through Editor C# builders, preserving existing scene references.'
  },
  art: {
    description: 'Creates coherent original art briefs and Meshy request manifests, and plans Unity AI authoring. Returns asset paths, budgets and review criteria.',
    scopes: ['content/art', 'docs/art'],
    focus: 'Use copper forest, weathered stone and turquoise magic. Create original assets. Specify scale, silhouette, polycount, pivot, materials and provenance. You can query Meshy balance and task status; paid generation is a separate CLI step.'
  },
  qa: {
    description: 'Reviews changes and runs Unity compile/scene validation. Returns pass/fail evidence, reproducible bugs and remaining gaps.',
    scopes: ['docs/qa', 'game/Assets/Yendor/Tests'],
    focus: 'Distinguish compilation, automated runtime checks, visual inspection and playtesting. Never infer a pass from absent output.'
  },
  design: {
    description: 'Develops the game design, milestone acceptance criteria and domain language. Returns focused specs and tradeoffs.',
    scopes: ['docs/design'],
    focus: 'Protect the combination of systemic discovery, beautiful exploration and fast shooter combat. Design one testable vertical slice at a time.'
  }
};

export function modelFor(role, env = process.env) {
  const profile = { combat: 'CODE', systems: 'CODE', world: 'CODE', art: 'GENERAL', design: 'REASONING', qa: 'REVIEW', supervisor: 'REASONING' }[role];
  const alias = env[`MODEL_${role.toUpperCase()}`] || env[`MODEL_${profile}`] || env.MODEL_DEFAULT;
  if (!alias) throw new Error(`Set MODEL_${role.toUpperCase()} or MODEL_DEFAULT to a LiteLLM model alias`);
  if (!env.LITELLM_API_KEY) throw new Error('Set LITELLM_API_KEY in the local .env');
  return { providerId: 'litellm', modelId: alias, url: env.LITELLM_BASE_URL || 'http://127.0.0.1:4000/v1', apiKey: env.LITELLM_API_KEY, api: 'chat' };
}
