interface LogoProps {
	size?: number;
}

/** Olevin logo. */
const Logo = ({ size = 26 }: Readonly<LogoProps>) => {
	return (
		<svg
			width={size}
			height={size}
			viewBox="0 0 512 512"
			fill="none"
			aria-hidden="true"
		>
			<circle cx="256" cy="256" r="256" fill="#5C4D9E" />
			<g transform="translate(256 256) scale(1.2) translate(-256 -256)">
				<path
					d="M149.93 362.07 A150 150 0 1 1 362.07 362.07"
					stroke="#FAF7F2"
					strokeOpacity={0.22}
					strokeWidth={36}
					strokeLinecap="round"
				/>
				<path
					d="M149.93 362.07 A150 150 0 1 1 405.54 267.77"
					stroke="#FAF7F2"
					strokeWidth={36}
					strokeLinecap="round"
				/>
				<path d="M190 330 A66 66 0 0 1 322 330 Z" fill="#E6A47C" />
				<line
					x1="166"
					y1="330"
					x2="346"
					y2="330"
					stroke="#FAF7F2"
					strokeOpacity={0.45}
					strokeWidth={10}
					strokeLinecap="round"
				/>
				<polyline
					points="176,316 216,296 252,304 292,262 330,240"
					stroke="#FAF7F2"
					strokeWidth={14}
					strokeLinecap="round"
					strokeLinejoin="round"
				/>
				<circle
					cx="330"
					cy="240"
					r="16"
					fill="#E6A47C"
					stroke="#FAF7F2"
					strokeWidth={8}
				/>
			</g>
		</svg>
	);
};

export default Logo;
