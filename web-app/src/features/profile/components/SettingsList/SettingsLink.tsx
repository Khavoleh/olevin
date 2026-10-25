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
	isDocument?: boolean;
	icon?: ReactNode;
	onClick?: () => void;
}

/** Whole-row link to a page; an external one opens in a new tab, a document one (a route handler) loads without the client router. */
const SettingsLink = ({
	label,
	hint,
	href,
	isExternal = false,
	isDocument = false,
	icon = <CaretRightIcon size={16} />,
	onClick,
}: Readonly<SettingsLinkProps>) => {
	const Anchor = isDocument ? "a" : Link;

	return (
		<Anchor
			href={href}
			target={isExternal ? "_blank" : undefined}
			rel={isExternal ? "noopener noreferrer" : undefined}
			onClick={onClick}
			className={`${SETTINGS_ROW_CLASSES} transition-colors hover:bg-surface-hover active:bg-surface-sunken`}
		>
			<SettingsLabel label={label} hint={hint} />
			<span aria-hidden className="flex-none text-fg-muted">
				{icon}
			</span>
		</Anchor>
	);
};

export default SettingsLink;
