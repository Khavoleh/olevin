const BROWSERS: [RegExp, string][] = [
	[/Edg(A|iOS)?\//, "Edge"],
	[/OPR\/|Opera/, "Opera"],
	[/SamsungBrowser\//, "Samsung Internet"],
	[/Firefox\/|FxiOS\//, "Firefox"],
	[/Chrome\/|CriOS\//, "Chrome"],
	[/Safari\//, "Safari"],
];

const SYSTEMS: [RegExp, string][] = [
	[/iPhone/, "iPhone"],
	[/iPad/, "iPad"],
	[/Android/, "Android"],
	[/CrOS/, "ChromeOS"],
	[/Mac OS X|Macintosh/, "macOS"],
	[/Windows/, "Windows"],
	[/Linux/, "Linux"],
];

const find = (userAgent: string, names: [RegExp, string][]) =>
	names.find(([pattern]) => pattern.test(userAgent))?.[1];

/** Browser and system of a user agent, as "Chrome · macOS"; the fallback when neither is known. */
export const getDeviceName = (userAgent: string, fallback: string) =>
	[find(userAgent, BROWSERS), find(userAgent, SYSTEMS)]
		.filter(Boolean)
		.join(" · ") || fallback;
