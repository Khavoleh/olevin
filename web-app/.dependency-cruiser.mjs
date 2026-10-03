export default {
	forbidden: [
		{
			name: "no-circular",
			severity: "error",
			comment:
				"This dependency is part of a circular relationship. You might want to revise your solution (i.e. use dependency inversion, make sure the modules have a single responsibility)",
			from: {},
			to: {
				circular: true,
			},
		},
		{
			name: "no-non-package-json",
			severity: "error",
			comment:
				"This module depends on an npm package that isn't in the 'dependencies' section of your package.json. That's problematic as the package either (1) won't be available on live (2 - worse) will be available on live with an non-guaranteed version. Fix it by adding the package to the dependencies in your package.json.",
			from: {},
			to: {
				dependencyTypes: ["npm-no-pkg", "npm-unknown"],
			},
		},
		{
			name: "not-to-unresolvable",
			comment:
				"This module depends on a module that cannot be found ('resolved to disk'). If it's an npm module: add it to your package.json. In all other cases you likely already know what to do.",
			severity: "error",
			from: {},
			to: {
				couldNotResolve: true,
			},
		},
		{
			name: "no-duplicate-dep-types",
			comment:
				"Likely this module depends on an external ('npm') package that occurs more than once in your package.json i.e. bot as a devDependencies and in dependencies. This will cause maintenance problems later on.",
			severity: "error",
			from: {},
			to: {
				moreThanOneDependencyType: true,
				dependencyTypesNot: ["type-only"],
			},
		},
		{
			name: "not-to-dev-dep",
			severity: "error",
			comment:
				"This module depends on an npm package from the 'devDependencies' section of your package.json. It looks like something that ships to production, though. To prevent problems with npm packages that aren't there on production declare it (only!) in the 'dependencies' section of your package.json. If this module is development only - add it to the from.pathNot re of the not-to-dev-dep rule in the dependency-cruiser configuration",
			from: {
				path: "^(src)",
				pathNot: "[.](?:spec|test|unit)[.](?:js|mjs|cjs|jsx|ts|mts|cts|tsx)$",
			},
			to: {
				dependencyTypes: ["npm-dev"],
				dependencyTypesNot: ["type-only"],
				pathNot: ["node_modules/@types/"],
			},
		},
		{
			name: "optional-deps-used",
			severity: "info",
			comment:
				"This module depends on an npm package that is declared as an optional dependency in your package.json. As this makes sense in limited situations only, it's flagged here. If you use an optional dependency here by design - add an exception to your dependency-cruiser configuration.",
			from: {},
			to: {
				dependencyTypes: ["npm-optional"],
			},
		},
		{
			name: "peer-deps-used",
			comment:
				"This module depends on an npm package that is declared as a peer dependency in your package.json. This makes sense if your package is e.g. a plugin, but in other cases - maybe not so much. If the use of a peer dependency is intentional add an exception to your dependency-cruiser configuration.",
			severity: "error",
			from: {},
			to: {
				dependencyTypes: ["npm-peer"],
			},
		},
		{
			name: "no-imports-into-shared-from-outside",
			comment: "In @shared you can import only @shared",
			from: { path: "^src/shared" },
			to: { pathNot: "^src/shared" },
			severity: "error",
		},
		{
			name: "widgets-imports-restriction",
			comment: "In @widgets you can import only @widgets and @shared",
			from: { path: "^src/widgets/.+" },
			to: {
				path: "^(?!src/widgets|src/shared/[^/]+/index\\.ts).+",
				pathNot: "^src/shared/[^/]+/index\\.ts$",
			},
			severity: "error",
		},
		{
			name: "features-imports-restriction",
			comment:
				"In @features you can import only @features, @widgets and @shared",
			from: { path: "^src/features/.+" },
			to: {
				path: "^(?!src/features|src/widgets|src/shared/[^/]+/index\\.ts).+",
				pathNot: "^src/shared/[^/]+/index\\.ts$",
			},
			severity: "error",
		},
		{
			name: "app-imports-restriction",
			comment:
				"In src/app (Next.js routes) and src/proxy.ts you can import only src/app, @features, @widgets and @shared",
			from: { path: "^src/(app/|proxy\\.ts$)" },
			to: {
				path: "^(?!src/app|src/features/[^/]+/index\\.ts|src/widgets/[^/]+/index\\.ts|src/shared/[^/]+/index\\.ts).+",
				pathNot:
					"^(src/features/[^/]+/index\\.ts|src/widgets/[^/]+/index\\.ts|src/shared/[^/]+/index\\.ts)$",
			},
			severity: "error",
		},
	],
	options: {
		doNotFollow: {
			path: ["node_modules"],
		},
		tsPreCompilationDeps: true,
		tsConfig: {
			fileName: "tsconfig.json",
		},
		enhancedResolveOptions: {
			exportsFields: ["exports"],
			conditionNames: ["import", "require", "node", "default", "types"],
			mainFields: ["module", "main", "types", "typings"],
		},
		skipAnalysisNotInRules: true,
		reporterOptions: {
			dot: {
				collapsePattern: "node_modules/(?:@[^/]+/[^/]+|[^/]+)",
			},
			archi: {
				collapsePattern:
					"^(?:packages|src|lib(s?)|app(s?)|bin|test(s?)|spec(s?))/[^/]+|node_modules/(?:@[^/]+/[^/]+|[^/]+)",
			},
			text: {
				highlightFocused: true,
			},
		},
		includeOnly: "^src",
	},
};
