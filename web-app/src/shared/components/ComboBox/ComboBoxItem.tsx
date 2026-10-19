"use client";

import {
	composeRenderProps,
	ListBoxItem,
	type ListBoxItemProps,
} from "react-aria-components";

const STATE_CLASSES = {
	idle: "",
	focused: "bg-surface-hover",
	selected: "bg-primary-soft font-semibold",
};

/** Option of a `ComboBox`. */
const ComboBoxItem = <T extends object>({
	className,
	...props
}: Readonly<ListBoxItemProps<T>>) => {
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

					return `cursor-pointer rounded-sm px-3 py-2 text-fg outline-none ${colors} ${customClasses ?? ""}`;
				},
			)}
		/>
	);
};

export default ComboBoxItem;
