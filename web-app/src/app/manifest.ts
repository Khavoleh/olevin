import { DEFAULT_LANGUAGE } from "@shared/constants";
import { getI18n } from "@shared/helpers";
import { APP_I18N } from "@shared/i18n";
import type { MetadataRoute } from "next";

/** Web app manifest that lets the app be installed and get push on iPhone. */
export default function manifest(): MetadataRoute.Manifest {
	const t = getI18n(DEFAULT_LANGUAGE, APP_I18N);

	return {
		id: "/",
		name: t("title"),
		short_name: t("title"),
		description: t("description"),
		start_url: "/",
		scope: "/",
		display: "standalone",
		background_color: "#F5F4FD",
		theme_color: "#5C4D9E",
		icons: [
			{ src: "/icons/icon-192.png", sizes: "192x192", type: "image/png" },
			{ src: "/icons/icon-512.png", sizes: "512x512", type: "image/png" },
			{
				src: "/icons/icon-maskable-512.png",
				sizes: "512x512",
				type: "image/png",
				purpose: "maskable",
			},
		],
	};
}
