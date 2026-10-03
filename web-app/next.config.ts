import type { NextConfig } from "next";

const nextConfig: NextConfig = {
	output: "standalone",
	poweredByHeader: false,
	cacheComponents: true,
};

export default nextConfig;
