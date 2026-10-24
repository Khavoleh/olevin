import { CaretRightIcon } from "@phosphor-icons/react/dist/ssr";
import Link from "next/link";
import type { ReactNode } from "react";
import SettingsLabel from "./SettingsLabel";
import { SETTINGS_ROW_CLASSES } from "./settings-row-classes";

interface SettingsLinkProps {
	label: string;
	hint?: string;
	href: string;
	isExternal?: boolean;
	icon?: ReactNode;
}

/** Whole-row link to a page; an external one opens in a new tab. */
const SettingsLink = ({
	label,
	hint,
	href,
	isExternal = false,
	icon = <CaretRightIcon size={16} />,
}: Readonly<SettingsLinkProps>) => (
	<Link
		href={href}
		target={isExternal ? "_blank" : undefined}
		rel={isExternal ? "noopener noreferrer" : undefined}
		className={`${SETTINGS_ROW_CLASSES} transition-colors hover:bg-surface-hover active:bg-surface-sunken`}
	>
		<SettingsLabel label={label} hint={hint} />
		<span aria-hidden className="flex-none text-fg-muted">
			{icon}
		</span>
	</Link>
);

export default SettingsLink;
