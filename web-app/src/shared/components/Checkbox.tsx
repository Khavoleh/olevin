"use client";

import { CheckIcon } from "@phosphor-icons/react";
import type { ReactNode } from "react";
import {
	CheckboxButton,
	CheckboxField,
	type CheckboxFieldProps,
} from "react-aria-components";
import FieldError from "./FieldError";
import Text from "./Text";

interface CheckboxProps
	extends Omit<CheckboxFieldProps, "className" | "children"> {
	description?: string;
	errorMessage?: string;
	className?: string;
	children: ReactNode;
}

const STATE_CLASSES = {
	idle: "border-input bg-surface",
	hovered: "border-input-hover bg-surface",
	selected: "border-primary bg-primary text-on-primary",
};

/** Checkbox with its label, hint and validation error. */
const Checkbox = ({
	description,
	errorMessage,
	className = "",
	children,
	...props
}: Readonly<CheckboxProps>) => {
	return (
		<CheckboxField {...props} className={`grid gap-1 ${className}`}>
			<CheckboxButton className="flex cursor-pointer items-start gap-3 outline-none">
				{({ isSelected, isHovered, isFocusVisible }) => {
					const box =
						(isSelected && STATE_CLASSES.selected) ||
						(isHovered && STATE_CLASSES.hovered) ||
						STATE_CLASSES.idle;

					return (
						<>
							<span
								aria-hidden="true"
								data-focus-visible={isFocusVisible || undefined}
								className={`mt-0.5 flex size-5 flex-none items-center justify-center rounded-sm border-2 transition-colors ${box}`}
							>
								{isSelected && <CheckIcon size={14} weight="bold" />}
							</span>
							<span>{children}</span>
						</>
					);
				}}
			</CheckboxButton>
			{description && (
				<Text slot="description" variant="hint" className="pl-8">
					{description}
				</Text>
			)}
			<FieldError className="pl-8">{errorMessage}</FieldError>
		</CheckboxField>
	);
};

export default Checkbox;
