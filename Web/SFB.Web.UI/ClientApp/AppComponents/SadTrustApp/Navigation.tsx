import React from 'react';
import NavItem from "./NavItem";

export type navigationItem = {
  Key: string,
  Value: string,
};

interface Props {
  navigationItems: navigationItem[];
  onClick?: () => void;
  hasKs2Progress: boolean;
  hasProgress8: boolean;
}

export default function Navigation(props: Props) {
  const items = props.navigationItems.filter((item) => {
    if ((item.Key === 'Ks2Score' && !props.hasKs2Progress) || (!props.hasProgress8 && item.Key === 'Progress8Score')) {
      return false;
    }
    return item;
  });
 
  return (
    <nav className="sfb-subnav">
      <ul className="sfb-subnav__section">
        {items.map((item) => (
          <NavItem key={item.Key}
                   linkTarget={item.Key}
                   linkText={item.Value} onClick={props.onClick}/>
        ))}
      </ul>
    </nav>
  );
}