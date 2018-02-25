import * as React from 'react';
import { NavigationMenuComponent } from './NavigationMenuComponent';

export interface LayoutProps {
	children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
	public render() {
		return <div className='container-fluid'>
			<div className='row'>
				<div className='col-sm-3'>
					<NavigationMenuComponent />
				</div>
				<div className='col-sm-9'>
					{ this.props.children }
				</div>
			</div>
		</div>;
	}
}