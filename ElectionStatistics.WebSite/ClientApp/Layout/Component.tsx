import * as React from 'react';

export interface LayoutProps {
	children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps> {
	constructor(props: LayoutProps) {
        super(props);
	}

	public render() {
		return (
			<div className='container-fluid'>
				<div className='row'>
					{ this.props.children }
				</div>
			</div>
		);
	}
}