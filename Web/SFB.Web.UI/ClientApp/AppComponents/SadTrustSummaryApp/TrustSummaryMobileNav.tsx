import { Menu, Button, Text } from '@mantine/core';
import {SyntheticEvent} from "react";

interface Props {
  navItems: string[]
  currentCategory: string
  handleCategoryChange: (e: SyntheticEvent, category: string) => void
}

export default function TrustSummaryMobileNav({navItems, currentCategory, handleCategoryChange} : Props) {
  return (
    <Menu shadow="md" width={200}>
      <Menu.Target>
        <Button variant="subtle" color="#1d70b8" className="sfb-trust-sad__mobile-nav-launcher govuk-link" compact>
          {currentCategory}: Change</Button>
      </Menu.Target>

      <Menu.Dropdown>
        {navItems.map(item => (
          <Menu.Item
            key={`mbnav-${item}`}
            onClick={(e: SyntheticEvent<Element, Event>)=> handleCategoryChange(e, item)}>{item}</Menu.Item>
        ))}
      </Menu.Dropdown>
    </Menu>
  );
}