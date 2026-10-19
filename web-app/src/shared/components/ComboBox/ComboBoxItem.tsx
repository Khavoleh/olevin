"use client";

import type { ReactNode } from "react";
import {
	composeRenderProps,
	ListBoxItem,
	type ListBoxItemProps,
} from "react-aria-components";
import Text from "../Text";

interface ComboBoxItemProps<T extends object>
	extends Omit<ListBoxItemProps<T>, "children"> {
	description?: string;
	children: ReactNode;
}

const STATE_CLASSES = {
	idle: "",
	focused: "bg-surface-hover",
	selected: "bg-primary-soft font-semibold",
};

/** Option of a `ComboBox` with an optional hint under its label. */
const ComboBoxItem = <T extends object>({
	description,
	className,
	children,
	...props
}: Readonly<ComboBoxItemProps<T>>) => {
	return (
		<ListBoxItem
			{...props}
			className={composeRenderProps(
				className,
				(customClasses, { isFocused, isSelected }) => {
					const colors =
						(isSelected && STATE_CLASSES.selected) ||
						(isFocused && STATE_CLASSES.focused) ||
						STATE_CLASSES.idle;

					return `grid cursor-pointer rounded-sm px-3 py-2 text-fg outline-none ${colors} ${customClasses ?? ""}`;
				},
			)}
		>
			<Text slot="label" elementType="span">
				{children}
			</Text>
			{description && (
				<Text
					slot="description"
					variant="hint"
					elementType="span"
					className="font-normal"
				>
					{description}
				</Text>
			)}
		</ListBoxItem>
	);
};

export default ComboBoxItem;
