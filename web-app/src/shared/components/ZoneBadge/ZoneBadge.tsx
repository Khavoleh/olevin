import { getI18n } from "@shared/helpers";
import { ZONE_BADGE_I18N } from "./zone-badge-i18n";

type Zone = "Stress" | "Unstable" | "Calm";

interface ZoneBadgeProps {
	zone: Zone;
	language: string;
	size?: "md" | "sm";
}

const ZONE_CLASSES: Record<Zone, string> = {
	Stress: "bg-zone-stress-soft text-zone-stress",
	Unstable: "bg-zone-unstable-soft text-zone-unstable",
	Calm: "bg-zone-calm-soft text-zone-calm",
};

const SIZE_CLASSES = {
	md: "h-6.5 px-2.75 text-sm",
	sm: "h-5.5 px-2 text-xs",
};

/** Rounded chip with the name of an index zone in its colors. */
const ZoneBadge = ({
	zone,
	language,
	size = "md",
}: Readonly<ZoneBadgeProps>) => {
	const t = getI18n(language, ZONE_BADGE_I18N);

	return (
		<span
			className={`inline-flex items-center whitespace-nowrap rounded-full font-semibold ${SIZE_CLASSES[size]} ${ZONE_CLASSES[zone]}`}
		>
			{t(zone)}
		</span>
	);
};

export default ZoneBadge;
