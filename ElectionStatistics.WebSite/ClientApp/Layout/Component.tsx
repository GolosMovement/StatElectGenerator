import * as React from 'react';
import { NavigationMenuComponent } from './NavigationMenuComponent';

export interface LayoutProps {
	children?: React.ReactNode;
}

export interface LayoutState {
	isInIframe: boolean;
}
	
function isInIframe() {
	try {
		return window.self !== window.top;
	} catch (e) {
		return true;
	}
}

export class Layout extends React.Component<LayoutProps, LayoutState> {
	constructor(props: LayoutProps) {
        super(props);

        this.state = {
			isInIframe: isInIframe() 
		}
	}

	public render() {
		if (this.state.isInIframe) {		
			return (
				<div className='container-fluid'>
					<div className='row'>
						{ this.props.children }
					</div>
				</div>
			);
		}
		else {			
			return (
				<div className='container-fluid'>
					<div className='row'>
						<div className='col-sm-3'>
							<NavigationMenuComponent />
						</div>
						<div className='col-sm-9'>
							{ this.props.children }
						</div>
					</div>
				</div>
			);
		}
	}
}