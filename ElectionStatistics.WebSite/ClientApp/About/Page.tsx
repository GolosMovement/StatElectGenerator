import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';

export class AboutPage extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
            <h1>О сайте</h1>
            <p>
                Данный сайт предоставляет возможность проводить разнообразные выборки на основании данных о голосовании на по выборах.
                Все данные взяты с сайта <a href="http://www.vybory.izbirkom.ru" target="_blank">ЦИК России</a>.
            </p>
        </div>;
    }
}
