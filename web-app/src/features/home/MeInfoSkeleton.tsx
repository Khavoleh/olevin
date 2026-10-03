/** Same height as the loaded text, so the card does not jump when the user arrives. */
const MeInfoSkeleton = () => {
	return (
		<div className="mt-2 flex min-h-6 items-center" aria-hidden="true">
			<span className="h-4 w-2/3 animate-pulse rounded bg-muted" />
		</div>
	);
};

export default MeInfoSkeleton;
