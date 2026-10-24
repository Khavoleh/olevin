"use client";

import type { ReactNode } from "react";
import { Dialog, Modal, ModalOverlay } from "react-aria-components";
import Button from "./Button";
import Heading from "./Heading";

interface ConfirmDialogProps {
	title: string;
	children: ReactNode;
	confirmLabel: string;
	cancelLabel: string;
	variant?: "primary" | "danger";
	isOpen: boolean;
	isPending?: boolean;
	onOpenChange: (isOpen: boolean) => void;
	onConfirm: () => void;
}

/** Modal that asks to confirm an action; it stays open while the action runs. */
const ConfirmDialog = ({
	title,
	children,
	confirmLabel,
	cancelLabel,
	variant = "primary",
	isOpen,
	isPending = false,
	onOpenChange,
	onConfirm,
}: Readonly<ConfirmDialogProps>) => {
	return (
		<ModalOverlay
			isOpen={isOpen}
			onOpenChange={onOpenChange}
			isDismissable={!isPending}
			isKeyboardDismissDisabled={isPending}
			className="fixed inset-0 z-30 flex items-end justify-center bg-overlay p-4 data-entering:animate-popover-in data-exiting:animate-popover-out md:items-center"
		>
			<Modal className="w-full max-w-md rounded-xl bg-surface p-5 shadow-overlay">
				<Dialog role="alertdialog" className="grid gap-4 outline-none">
					<Heading slot="title" level={2}>
						{title}
					</Heading>
					<div className="grid gap-2">{children}</div>
					<div className="flex flex-col-reverse gap-2 md:flex-row md:justify-end">
						<Button
							variant="secondary"
							isDisabled={isPending}
							onPress={() => onOpenChange(false)}
						>
							{cancelLabel}
						</Button>
						<Button variant={variant} isPending={isPending} onPress={onConfirm}>
							{confirmLabel}
						</Button>
					</div>
				</Dialog>
			</Modal>
		</ModalOverlay>
	);
};

export default ConfirmDialog;
