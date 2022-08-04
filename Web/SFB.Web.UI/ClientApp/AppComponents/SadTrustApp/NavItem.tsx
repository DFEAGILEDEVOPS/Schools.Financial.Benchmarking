import React from 'react';
import { NavLink } from 'react-router-dom';

interface Props {
  linkTarget: string;
  linkText: string;
  onClick?: () => void;
}

export default function NavItem(props: Props) {
  const activeClassNames = 'sfb-subnav__item--current sfb-subnav__link govuk-link govuk-link--no-visited-state govuk-link--no-underline';
  const containerClasses = 'sfb-subnav__item';
  const linkClasses = 'sfb-subnav__link govuk-link govuk-link--no-visited-state govuk-link--no-underline';
  return (
      <li className={containerClasses}>
        <NavLink
          className={({ isActive}) => isActive ? activeClassNames : linkClasses }
          to={props.linkTarget} onClick={props.onClick}>{props.linkText}</NavLink>
      </li>
    )
 
}